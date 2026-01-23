using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.API.Internal;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Internal;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class GroupTravelService : IGroupTravelService
    {
        private readonly IGroupTravelRequestRepository _requestRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IPaymentRecordRepository _paymentRecordRepository;
        private readonly ITourInfoService _tourInfoService;
        private readonly IUserInfoService _userInfoService;
        private readonly INotificationServiceInternal _notificationService;
        private readonly IMapper _mapper;

        public GroupTravelService(
            IGroupTravelRequestRepository requestRepository,
            IWalletRepository walletRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IPaymentRecordRepository paymentRecordRepository,
            ITourInfoService tourInfoService,
            IUserInfoService userInfoService,
            INotificationServiceInternal notificationService,
            IMapper mapper)
        {
            _requestRepository = requestRepository;
            _walletRepository = walletRepository;
            _tokenRepository = tokenRepository;
            _paymentRecordRepository = paymentRecordRepository;
            _tourInfoService = tourInfoService;
            _userInfoService = userInfoService;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public GroupTravelRequestDto Create(int organizerId, CreateGroupTravelRequestDto dto)
        {
            var tourInfo = _tourInfoService.Get(dto.TourId);
            if (tourInfo == null)
                throw new NotFoundException($"Tour {dto.TourId} not found.");

            var participantIds = new List<int>();
            
            var organizerUser = _userInfoService.GetUser(organizerId);
            if (organizerUser == null)
                throw new NotFoundException("User not found.");

            foreach (var username in dto.ParticipantEmails)
            {
                var user = _userInfoService.GetUserByUsername(username);
                
                if (user == null || !user.IsActive || user.IsAdministrator)
                    throw new NotFoundException($"User '{username}' not found.");

                if (user.Id == organizerId)
                    throw new ArgumentException("You cannot add yourself as a participant.");

                var participantId = (int)user.Id;
                if (participantIds.Contains(participantId))
                    throw new ArgumentException($"Duplicate participant: {username}");

                participantIds.Add(participantId);
            }

            var request = new GroupTravelRequest(
                organizerId,
                dto.TourId,
                tourInfo.Name,
                tourInfo.Price,
                participantIds
            );

            var created = _requestRepository.Create(request);

            foreach (var participantId in participantIds)
            {
                _notificationService.CreateMessageNotification(
                    userId: participantId,
                    actorId: organizerId,
                    actorUsername: organizerUser?.Username ?? "User",
                    content: $"{organizerUser?.Username ?? "A user"} invited you to a group travel: {tourInfo.Name} (Price: {tourInfo.Price} AC)",
                    resourceUrl: $"/group-travel/requests/{created.Id}"
                );
            }

            return MapToDto(created, organizerId);
        }

        public List<GroupTravelRequestDto> GetMyRequests(int touristId)
        {
            var requests = _requestRepository.GetByOrganizerId(touristId);
            return requests.Select(r => MapToDto(r, touristId)).ToList();
        }

        public List<GroupTravelRequestDto> GetRequestsForMe(int touristId)
        {
            var requests = _requestRepository.GetByParticipantId(touristId);
            return requests.Select(r => MapToDto(r, touristId)).ToList();
        }

        public GroupTravelRequestDto AcceptRequest(int requestId, int participantId)
        {
            var request = _requestRepository.GetById(requestId);
            if (request == null)
                throw new NotFoundException($"Request {requestId} not found.");

            var participant = request.Participants.FirstOrDefault(p => p.TouristId == participantId);
            if (participant == null)
                throw new ForbiddenException("You are not a participant in this request.");

            var wallet = _walletRepository.GetByTouristId(participantId);
            if (wallet == null)
            {
                wallet = new Wallet(participantId);
                wallet = _walletRepository.Create(wallet);
            }

            if (wallet.Balance < request.PricePerPerson)
                throw new InvalidOperationException($"Insufficient balance. Required: {request.PricePerPerson} AC, Available: {wallet.Balance} AC");

            wallet.DeductBalance(request.PricePerPerson);
            _walletRepository.Update(wallet);

            var organizerWallet = _walletRepository.GetByTouristId(request.OrganizerId);
            if (organizerWallet == null)
            {
                organizerWallet = new Wallet(request.OrganizerId);
                organizerWallet = _walletRepository.Create(organizerWallet);
            }
            organizerWallet.AddBalance(request.PricePerPerson);
            _walletRepository.Update(organizerWallet);

            request.AcceptParticipant(participantId);
            var updated = _requestRepository.Update(request);

            var participantUser = _userInfoService.GetUser(participantId);
            var organizerUser = _userInfoService.GetUser(request.OrganizerId);

            if (participantUser != null && organizerUser != null)
            {
                _notificationService.CreateMessageNotification(
                    userId: request.OrganizerId,
                    actorId: participantId,
                    actorUsername: participantUser.Username,
                    content: $"{participantUser.Username} accepted your group travel request: {request.TourName}",
                    resourceUrl: $"/group-travel/requests/{request.Id}"
                );
            }

            return MapToDto(updated, participantId);
        }

        public GroupTravelRequestDto RejectRequest(int requestId, int participantId)
        {
            var request = _requestRepository.GetById(requestId);
            if (request == null)
                throw new NotFoundException($"Request {requestId} not found.");

            var participant = request.Participants.FirstOrDefault(p => p.TouristId == participantId);
            if (participant == null)
                throw new ForbiddenException("You are not a participant in this request.");

            request.RejectParticipant(participantId);
            var updated = _requestRepository.Update(request);

            var participantUser = _userInfoService.GetUser(participantId);
            var organizerUser = _userInfoService.GetUser(request.OrganizerId);

            if (participantUser != null && organizerUser != null)
            {
                _notificationService.CreateMessageNotification(
                    userId: request.OrganizerId,
                    actorId: participantId,
                    actorUsername: participantUser.Username,
                    content: $"{participantUser.Username} rejected your group travel request: {request.TourName}",
                    resourceUrl: $"/group-travel/requests/{request.Id}"
                );
            }

            return MapToDto(updated, participantId);
        }

        public List<TourPurchaseTokenDto> CompleteRequest(int requestId, int organizerId)
        {
            var request = _requestRepository.GetById(requestId);
            if (request == null)
                throw new NotFoundException($"Request {requestId} not found.");

            if (request.OrganizerId != organizerId)
                throw new ForbiddenException("Only organizer can complete the request.");

            if (request.Status != GroupTravelStatus.Accepted)
                throw new InvalidOperationException("Cannot complete request that is not accepted by all participants.");

            var organizerWallet = _walletRepository.GetByTouristId(organizerId);
            if (organizerWallet == null)
            {
                organizerWallet = new Wallet(organizerId);
                organizerWallet = _walletRepository.Create(organizerWallet);
            }

            var totalPrice = request.PricePerPerson * (1 + request.Participants.Count);
            if (organizerWallet.Balance < totalPrice)
                throw new InvalidOperationException($"Insufficient balance. Required: {totalPrice} AC, Available: {organizerWallet.Balance} AC");

            organizerWallet.DeductBalance(totalPrice);
            _walletRepository.Update(organizerWallet);

            var createdTokens = new List<TourPurchaseTokenDto>();

            var organizerToken = new TourPurchaseToken(organizerId, request.TourId);
            var savedOrganizerToken = _tokenRepository.Create(organizerToken);
            createdTokens.Add(_mapper.Map<TourPurchaseTokenDto>(savedOrganizerToken));

            var organizerPayment = new PaymentRecord(organizerId, request.TourId, request.PricePerPerson);
            _paymentRecordRepository.Create(organizerPayment);

            foreach (var participantId in request.GetAcceptedParticipantIds())
            {
                var participantToken = new TourPurchaseToken(participantId, request.TourId);
                var savedParticipantToken = _tokenRepository.Create(participantToken);
                createdTokens.Add(_mapper.Map<TourPurchaseTokenDto>(savedParticipantToken));

                var participantPayment = new PaymentRecord(participantId, request.TourId, request.PricePerPerson);
                _paymentRecordRepository.Create(participantPayment);
            }

            request.Complete();
            _requestRepository.Update(request);

            return createdTokens;
        }

        public void CancelRequest(int requestId, int organizerId)
        {
            var request = _requestRepository.GetById(requestId);
            if (request == null)
                throw new NotFoundException($"Request {requestId} not found.");

            if (request.OrganizerId != organizerId)
                throw new ForbiddenException("Only organizer can cancel the request.");

            if (request.Status == GroupTravelStatus.Completed)
                throw new InvalidOperationException("Cannot cancel completed request.");

            var organizerWallet = _walletRepository.GetByTouristId(organizerId);
            foreach (var participantId in request.GetAcceptedParticipantIds())
            {
                var participantWallet = _walletRepository.GetByTouristId(participantId);
                if (participantWallet != null && organizerWallet != null)
                {
                    participantWallet.AddBalance(request.PricePerPerson);
                    _walletRepository.Update(participantWallet);

                    organizerWallet.DeductBalance(request.PricePerPerson);
                    _walletRepository.Update(organizerWallet);
                }
            }

            request.Cancel();
            _requestRepository.Update(request);
        }

        private GroupTravelRequestDto MapToDto(GroupTravelRequest request, int currentUserId)
        {
            var organizerUser = _userInfoService.GetUser(request.OrganizerId);

            var participants = new List<GroupTravelParticipantDto>();
            foreach (var participant in request.Participants)
            {
                var participantUser = _userInfoService.GetUser(participant.TouristId);

                participants.Add(new GroupTravelParticipantDto
                {
                    TouristId = participant.TouristId,
                    TouristUsername = participantUser?.Username ?? "Unknown",
                    Status = (int)participant.Status,
                    RespondedAt = participant.RespondedAt
                });
            }

            return new GroupTravelRequestDto
            {
                Id = (int)request.Id,
                OrganizerId = request.OrganizerId,
                OrganizerUsername = organizerUser?.Username ?? "Unknown",
                TourId = request.TourId,
                TourName = request.TourName,
                PricePerPerson = request.PricePerPerson,
                Status = (int)request.Status,
                CreatedAt = request.CreatedAt,
                CompletedAt = request.CompletedAt,
                Participants = participants,
                CanComplete = request.Status == GroupTravelStatus.Accepted && request.OrganizerId == currentUserId
            };
        }
    }
}

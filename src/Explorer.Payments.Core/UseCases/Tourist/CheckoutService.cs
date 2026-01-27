using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.API.Internal;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Internal;

namespace Explorer.Payments.Core.UseCases.Tourist
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IPaymentRecordRepository _paymentRecordRepository;
        private readonly ITourInfoService _tourInfoService;
        private readonly IBundlePurchaseService _bundlePurchaseService;
        private readonly IGroupTravelRequestRepository _groupTravelRequestRepository;
        private readonly INotificationServiceInternal _notificationService;
        private readonly IUserInfoService _userInfoService;
        private readonly IMapper _mapper;

        public CheckoutService(
            IShoppingCartRepository cartRepository,
            ITourPurchaseTokenRepository tokenRepository,
            IWalletRepository walletRepository,
            IPaymentRecordRepository paymentRecordRepository,
            ITourInfoService tourInfoService,
            IBundlePurchaseService bundlePurchaseService,
            IGroupTravelRequestRepository groupTravelRequestRepository,
            INotificationServiceInternal notificationService,
            IUserInfoService userInfoService,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tokenRepository = tokenRepository;
            _walletRepository = walletRepository;
            _paymentRecordRepository = paymentRecordRepository;
            _tourInfoService = tourInfoService;
            _bundlePurchaseService = bundlePurchaseService;
            _groupTravelRequestRepository = groupTravelRequestRepository;
            _notificationService = notificationService;
            _userInfoService = userInfoService;
            _mapper = mapper;
        }

        public List<TourPurchaseTokenDto> Checkout(int touristId)
        {
            var cart = _cartRepository.GetByTouristId(touristId);
            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                throw new System.InvalidOperationException("Shopping cart is empty.");
            }

            var wallet = _walletRepository.GetByTouristId(touristId);
            if (wallet == null)
            {
                wallet = new Wallet(touristId);
                wallet = _walletRepository.Create(wallet);
            }

            var tourItemsToPurchase = new List<OrderItem>();
            var bundleItemsToPurchase = new List<OrderItem>();
            var groupTravelItems = new List<(OrderItem Item, GroupTravelRequest Request)>();

            foreach (var item in cart.Items)
            {
                if (item.BundleId.HasValue)
                {
                    if (_paymentRecordRepository.ExistsForBundle(touristId, item.BundleId.Value)) continue;
                    bundleItemsToPurchase.Add(item);
                }
                else
                {
                    if (_tokenRepository.Exists(touristId, item.TourId)) continue;

                    var groupTravelRequest = _groupTravelRequestRepository.GetByOrganizerId(touristId)
                        .FirstOrDefault(r => r.TourId == item.TourId && r.Status != GroupTravelStatus.Completed && r.Status != GroupTravelStatus.Cancelled);

                    if (groupTravelRequest != null)
                    {
                        groupTravelItems.Add((item, groupTravelRequest));
                        continue;
                    }

                    tourItemsToPurchase.Add(item);
                }
            }

            if (tourItemsToPurchase.Count == 0 && bundleItemsToPurchase.Count == 0 && groupTravelItems.Count == 0)
            {
                cart.Clear();
                _cartRepository.Update(cart);
                return new List<TourPurchaseTokenDto>();
            }

            decimal tourTotalPrice = tourItemsToPurchase.Sum(item => item.Price);
            decimal bundleTotalPrice = bundleItemsToPurchase.Sum(item => item.Price);
            decimal groupTravelTotalPrice = 0;
            foreach (var (item, groupTravelRequest) in groupTravelItems)
            {
                var acceptedParticipantIds = groupTravelRequest.GetAcceptedParticipantIds();
                var totalParticipants = 1 + acceptedParticipantIds.Count;
                groupTravelTotalPrice += groupTravelRequest.PricePerPerson * totalParticipants;
            }
            decimal totalPrice = tourTotalPrice + bundleTotalPrice + groupTravelTotalPrice;

            if (wallet.Balance < totalPrice)
            {
                throw new System.InvalidOperationException($"Insufficient balance. Required: {totalPrice} AC, Available: {wallet.Balance} AC");
            }

            if (totalPrice > 0)
            {
                wallet.DeductBalance(totalPrice);
                _walletRepository.Update(wallet);
            }

            var createdTokens = new List<TourPurchaseTokenDto>();

            foreach (var (item, groupTravelRequest) in groupTravelItems)
            {
                var acceptedParticipantIds = groupTravelRequest.GetAcceptedParticipantIds();

                if (acceptedParticipantIds.Count == 0)
                {
                    var paymentRecord = new PaymentRecord(touristId, item.TourId, item.Price);
                    _paymentRecordRepository.Create(paymentRecord);

                    var token = new TourPurchaseToken(touristId, item.TourId);
                    var saved = _tokenRepository.Create(token);
                    createdTokens.Add(_mapper.Map<TourPurchaseTokenDto>(saved));
                    continue;
                }

                var organizerToken = new TourPurchaseToken(touristId, item.TourId);
                var savedOrganizerToken = _tokenRepository.Create(organizerToken);
                createdTokens.Add(_mapper.Map<TourPurchaseTokenDto>(savedOrganizerToken));

                var organizerPayment = new PaymentRecord(touristId, item.TourId, groupTravelRequest.PricePerPerson);
                _paymentRecordRepository.Create(organizerPayment);

                var organizerUser = _userInfoService.GetUser(touristId);
                foreach (var participantId in acceptedParticipantIds)
                {
                    if (_tokenRepository.Exists(participantId, item.TourId)) continue;

                    var participantToken = new TourPurchaseToken(participantId, item.TourId);
                    var savedParticipantToken = _tokenRepository.Create(participantToken);
                    createdTokens.Add(_mapper.Map<TourPurchaseTokenDto>(savedParticipantToken));

                    var participantPayment = new PaymentRecord(participantId, item.TourId, groupTravelRequest.PricePerPerson);
                    _paymentRecordRepository.Create(participantPayment);

                    if (organizerUser != null)
                    {
                        _notificationService.CreateMessageNotification(
                            userId: participantId,
                            actorId: touristId,
                            actorUsername: organizerUser.Username,
                            content: $"{organizerUser.Username} has completed the group travel for tour '{groupTravelRequest.TourName}'. The tour is now in your collection!",
                            resourceUrl: "/tour-execution/purchased-tours"
                        );
                    }
                }

                groupTravelRequest.Complete();
                _groupTravelRequestRepository.Update(groupTravelRequest);
            }

            foreach (var item in tourItemsToPurchase)
            {
                var paymentRecord = new PaymentRecord(touristId, item.TourId, item.Price);
                _paymentRecordRepository.Create(paymentRecord);

                var token = new TourPurchaseToken(touristId, item.TourId);
                var saved = _tokenRepository.Create(token);
                createdTokens.Add(_mapper.Map<TourPurchaseTokenDto>(saved));
            }

            foreach (var item in bundleItemsToPurchase)
            {
                var bundleTokens = _bundlePurchaseService.PurchaseBundle(touristId, item.BundleId.Value);
                createdTokens.AddRange(bundleTokens);
            }

            cart.Clear();
            _cartRepository.Update(cart);

            return createdTokens;
        }

        public List<TourPurchaseTokenDto> GetPurchaseTokens(int touristId)
        {
            var tokens = _tokenRepository.GetByTouristId(touristId);
            return tokens.Select(t => _mapper.Map<TourPurchaseTokenDto>(t)).ToList();
        }
    }
}
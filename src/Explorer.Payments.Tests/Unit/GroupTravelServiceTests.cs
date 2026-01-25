using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.UseCases.Tourist;
using Explorer.Tours.API.Internal;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Unit
{
    public class GroupTravelServiceTests
    {
        private class GroupTravelRequestRepoStub : IGroupTravelRequestRepository
        {
            public List<GroupTravelRequest> Store = new();

            public GroupTravelRequest Create(GroupTravelRequest request)
            {
                Store.Add(request);
                return request;
            }

            public GroupTravelRequest Update(GroupTravelRequest request)
            {
                var existing = Store.FirstOrDefault(r => r.Id == request.Id);
                if (existing != null)
                {
                    Store.Remove(existing);
                }
                Store.Add(request);
                return request;
            }

            public GroupTravelRequest? GetById(int id)
            {
                return Store.FirstOrDefault(r => r.Id == id);
            }

            public List<GroupTravelRequest> GetByOrganizerId(int organizerId)
            {
                return Store.Where(r => r.OrganizerId == organizerId).ToList();
            }

            public List<GroupTravelRequest> GetByParticipantId(int participantId)
            {
                return Store.Where(r => r.Participants.Any(p => p.TouristId == participantId)).ToList();
            }

            public GroupTravelRequest? GetPendingByParticipantAndTour(int participantId, int tourId)
            {
                return Store.FirstOrDefault(r => r.TourId == tourId && 
                    r.Status == GroupTravelStatus.Pending && 
                    r.Participants.Any(p => p.TouristId == participantId));
            }
        }

        private class WalletRepoStub : IWalletRepository
        {
            public Dictionary<int, Wallet> Store = new();

            public Wallet? GetByTouristId(int touristId)
            {
                return Store.ContainsKey(touristId) ? Store[touristId] : null;
            }

            public Wallet Create(Wallet wallet)
            {
                Store[wallet.TouristId] = wallet;
                return wallet;
            }

            public Wallet Update(Wallet wallet)
            {
                Store[wallet.TouristId] = wallet;
                return wallet;
            }

            public bool Exists(int touristId)
            {
                return Store.ContainsKey(touristId);
            }
        }

        private class TokenRepoStub : ITourPurchaseTokenRepository
        {
            public List<TourPurchaseToken> Store = new();

            public TourPurchaseToken Create(TourPurchaseToken token)
            {
                Store.Add(token);
                return token;
            }

            public List<TourPurchaseToken> GetByTouristId(int touristId)
            {
                return Store.Where(t => t.TouristId == touristId).ToList();
            }

            public bool Exists(int touristId, int tourId)
            {
                return Store.Any(t => t.TouristId == touristId && t.TourId == tourId);
            }
        }

        private class PaymentRecordRepoStub : IPaymentRecordRepository
        {
            public List<PaymentRecord> Store = new();

            public PaymentRecord Create(PaymentRecord record)
            {
                Store.Add(record);
                return record;
            }

            public bool ExistsForBundle(int touristId, int bundleId)
            {
                return Store.Any(r => r.TouristId == touristId && r.BundleId == bundleId);
            }
        }

        private class TourInfoServiceStub : ITourInfoService
        {
            private readonly Dictionary<int, TourInfoDto> _tours = new();

            public void SetTour(int tourId, string name, decimal price)
            {
                _tours[tourId] = new TourInfoDto
                {
                    TourId = tourId,
                    Name = name,
                    Price = price,
                    Status = TourLifecycleStatus.Published
                };
            }

            public TourInfoDto Get(int tourId)
            {
                return _tours.ContainsKey(tourId) ? _tours[tourId] : null;
            }
        }

        private class UserInfoServiceStub : IUserInfoService
        {
            private readonly Dictionary<long, UserInfo> _users = new();
            private readonly Dictionary<string, UserInfo> _usersByUsername = new();

            public void AddUser(long id, string username, bool isActive = true, bool isAdministrator = false)
            {
                var user = new UserInfo
                {
                    Id = id,
                    Username = username,
                    IsActive = isActive,
                    IsAdministrator = isAdministrator
                };
                _users[id] = user;
                _usersByUsername[username] = user;
            }

            public UserInfo? GetUser(long userId)
            {
                return _users.ContainsKey(userId) ? _users[userId] : null;
            }

            public UserInfo? GetUserByUsername(string username)
            {
                return _usersByUsername.ContainsKey(username) ? _usersByUsername[username] : null;
            }

            public long? GetPersonIdByUsername(string username)
            {
                var u = GetUserByUsername(username);
                return u != null ? u.Id : null;
            }

            public bool IsAdministrator(long userId)
            {
                return _users.ContainsKey(userId) && _users[userId].IsAdministrator;
            }
        }

        private class NotificationServiceStub : INotificationServiceInternal
        {
            public List<(int userId, int actorId, string actorUsername, string content, string? resourceUrl)> Notifications = new();

            public void CreateMessageNotification(int userId, int actorId, string actorUsername, string content, string? resourceUrl)
            {
                Notifications.Add((userId, actorId, actorUsername, content, resourceUrl));
            }
        }

        private static IMapper Mapper()
        {
            var cfg = new MapperConfiguration(c =>
            {
                c.CreateMap<TourPurchaseToken, TourPurchaseTokenDto>().ReverseMap();
            });
            return cfg.CreateMapper();
        }

        [Fact]
        public void Create_creates_request_successfully()
        {
            var requestRepo = new GroupTravelRequestRepoStub();
            var walletRepo = new WalletRepoStub();
            var tokenRepo = new TokenRepoStub();
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            tourInfoService.SetTour(1, "Test Tour", 100m);
            var userInfoService = new UserInfoServiceStub();
            userInfoService.AddUser(1, "organizer");
            userInfoService.AddUser(2, "participant1");
            var notificationService = new NotificationServiceStub();

            var service = new GroupTravelService(
                requestRepo, walletRepo, tokenRepo, paymentRecordRepo,
                tourInfoService, userInfoService, notificationService, Mapper());

            var dto = new CreateGroupTravelRequestDto
            {
                TourId = 1,
                ParticipantEmails = new List<string> { "participant1" }
            };

            var result = service.Create(1, dto);

            result.ShouldNotBeNull();
            result.TourId.ShouldBe(1);
            result.OrganizerId.ShouldBe(1);
            result.PricePerPerson.ShouldBe(100m);
            result.Status.ShouldBe(0);
            result.Participants.Count.ShouldBe(1);
            requestRepo.Store.Count.ShouldBe(1);
            notificationService.Notifications.Count.ShouldBe(1);
        }

        [Fact]
        public void Create_throws_when_tour_not_found()
        {
            var requestRepo = new GroupTravelRequestRepoStub();
            var walletRepo = new WalletRepoStub();
            var tokenRepo = new TokenRepoStub();
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            var userInfoService = new UserInfoServiceStub();
            userInfoService.AddUser(1, "organizer");
            var notificationService = new NotificationServiceStub();

            var service = new GroupTravelService(
                requestRepo, walletRepo, tokenRepo, paymentRecordRepo,
                tourInfoService, userInfoService, notificationService, Mapper());

            var dto = new CreateGroupTravelRequestDto
            {
                TourId = 999,
                ParticipantEmails = new List<string> { "participant1" }
            };

            Should.Throw<NotFoundException>(() => service.Create(1, dto))
                .Message.ShouldContain("Tour 999 not found");
        }

        [Fact]
        public void Create_throws_when_participant_not_found()
        {
            var requestRepo = new GroupTravelRequestRepoStub();
            var walletRepo = new WalletRepoStub();
            var tokenRepo = new TokenRepoStub();
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            tourInfoService.SetTour(1, "Test Tour", 100m);
            var userInfoService = new UserInfoServiceStub();
            userInfoService.AddUser(1, "organizer");
            var notificationService = new NotificationServiceStub();

            var service = new GroupTravelService(
                requestRepo, walletRepo, tokenRepo, paymentRecordRepo,
                tourInfoService, userInfoService, notificationService, Mapper());

            var dto = new CreateGroupTravelRequestDto
            {
                TourId = 1,
                ParticipantEmails = new List<string> { "nonexistent" }
            };

            Should.Throw<NotFoundException>(() => service.Create(1, dto))
                .Message.ShouldContain("User 'nonexistent' not found");
        }

        [Fact]
        public void Create_throws_when_organizer_adds_himself()
        {
            var requestRepo = new GroupTravelRequestRepoStub();
            var walletRepo = new WalletRepoStub();
            var tokenRepo = new TokenRepoStub();
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            tourInfoService.SetTour(1, "Test Tour", 100m);
            var userInfoService = new UserInfoServiceStub();
            userInfoService.AddUser(1, "organizer");
            var notificationService = new NotificationServiceStub();

            var service = new GroupTravelService(
                requestRepo, walletRepo, tokenRepo, paymentRecordRepo,
                tourInfoService, userInfoService, notificationService, Mapper());

            var dto = new CreateGroupTravelRequestDto
            {
                TourId = 1,
                ParticipantEmails = new List<string> { "organizer" }
            };

            Should.Throw<ArgumentException>(() => service.Create(1, dto))
                .Message.ShouldContain("You cannot add yourself as a participant");
        }

        [Fact]
        public void AcceptRequest_transfers_funds_and_updates_status()
        {
            var requestRepo = new GroupTravelRequestRepoStub();
            var walletRepo = new WalletRepoStub();
            var wallet1 = new Wallet(1);
            wallet1.AddBalance(1000m);
            walletRepo.Create(wallet1);
            var wallet2 = new Wallet(2);
            wallet2.AddBalance(500m);
            walletRepo.Create(wallet2);
            var tokenRepo = new TokenRepoStub();
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            var userInfoService = new UserInfoServiceStub();
            userInfoService.AddUser(1, "organizer");
            userInfoService.AddUser(2, "participant1");
            var notificationService = new NotificationServiceStub();

            var request = new GroupTravelRequest(1, 1, "Test Tour", 100m, new List<int> { 2 });
            requestRepo.Create(request);

            var service = new GroupTravelService(
                requestRepo, walletRepo, tokenRepo, paymentRecordRepo,
                tourInfoService, userInfoService, notificationService, Mapper());

            var result = service.AcceptRequest((int)request.Id, 2);

            result.Status.ShouldBe(1);
            walletRepo.GetByTouristId(2).Balance.ShouldBe(400m);
            walletRepo.GetByTouristId(1).Balance.ShouldBe(1100m);
            notificationService.Notifications.Count.ShouldBe(1);
        }

        [Fact]
        public void AcceptRequest_throws_when_insufficient_balance()
        {
            var requestRepo = new GroupTravelRequestRepoStub();
            var walletRepo = new WalletRepoStub();
            var wallet1 = new Wallet(1);
            wallet1.AddBalance(1000m);
            walletRepo.Create(wallet1);
            var wallet2 = new Wallet(2);
            wallet2.AddBalance(50m);
            walletRepo.Create(wallet2);
            var tokenRepo = new TokenRepoStub();
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            var userInfoService = new UserInfoServiceStub();
            userInfoService.AddUser(1, "organizer");
            userInfoService.AddUser(2, "participant1");
            var notificationService = new NotificationServiceStub();

            var request = new GroupTravelRequest(1, 1, "Test Tour", 100m, new List<int> { 2 });
            requestRepo.Create(request);

            var service = new GroupTravelService(
                requestRepo, walletRepo, tokenRepo, paymentRecordRepo,
                tourInfoService, userInfoService, notificationService, Mapper());

            Should.Throw<InvalidOperationException>(() => service.AcceptRequest((int)request.Id, 2))
                .Message.ShouldContain("Insufficient balance");
        }

        [Fact]
        public void CompleteRequest_creates_tokens_for_all_participants()
        {
            var requestRepo = new GroupTravelRequestRepoStub();
            var walletRepo = new WalletRepoStub();
            var wallet1 = new Wallet(1);
            wallet1.AddBalance(1000m);
            walletRepo.Create(wallet1);
            var tokenRepo = new TokenRepoStub();
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            var userInfoService = new UserInfoServiceStub();
            var notificationService = new NotificationServiceStub();

            var request = new GroupTravelRequest(1, 1, "Test Tour", 100m, new List<int> { 2, 3 });
            request.AcceptParticipant(2);
            request.AcceptParticipant(3);
            requestRepo.Create(request);

            var service = new GroupTravelService(
                requestRepo, walletRepo, tokenRepo, paymentRecordRepo,
                tourInfoService, userInfoService, notificationService, Mapper());

            var result = service.CompleteRequest((int)request.Id, 1);

            result.Count.ShouldBe(3);
            tokenRepo.Store.Count.ShouldBe(3);
            tokenRepo.Store.Any(t => t.TouristId == 1 && t.TourId == 1).ShouldBeTrue();
            tokenRepo.Store.Any(t => t.TouristId == 2 && t.TourId == 1).ShouldBeTrue();
            tokenRepo.Store.Any(t => t.TouristId == 3 && t.TourId == 1).ShouldBeTrue();
            walletRepo.GetByTouristId(1).Balance.ShouldBe(700m);
            paymentRecordRepo.Store.Count.ShouldBe(3);
        }

        [Fact]
        public void CancelRequest_refunds_participants()
        {
            var requestRepo = new GroupTravelRequestRepoStub();
            var walletRepo = new WalletRepoStub();
            var wallet1 = new Wallet(1);
            wallet1.AddBalance(1000m);
            walletRepo.Create(wallet1);
            var wallet2 = new Wallet(2);
            wallet2.AddBalance(400m);
            walletRepo.Create(wallet2);
            var tokenRepo = new TokenRepoStub();
            var paymentRecordRepo = new PaymentRecordRepoStub();
            var tourInfoService = new TourInfoServiceStub();
            var userInfoService = new UserInfoServiceStub();
            var notificationService = new NotificationServiceStub();

            var request = new GroupTravelRequest(1, 1, "Test Tour", 100m, new List<int> { 2 });
            request.AcceptParticipant(2);
            requestRepo.Create(request);

            var service = new GroupTravelService(
                requestRepo, walletRepo, tokenRepo, paymentRecordRepo,
                tourInfoService, userInfoService, notificationService, Mapper());

            service.CancelRequest((int)request.Id, 1);

            var updatedRequest = requestRepo.GetById((int)request.Id);
            updatedRequest.Status.ShouldBe(GroupTravelStatus.Cancelled);
            walletRepo.GetByTouristId(2).Balance.ShouldBe(500m);
            walletRepo.GetByTouristId(1).Balance.ShouldBe(900m);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Tourist.Payments;
using Explorer.API.Hubs;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class GroupTravelCommandTests : BasePaymentsIntegrationTest
    {
        public GroupTravelCommandTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Create_creates_request_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var stakeholdersContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            
            EnsureTestTourExists(toursContext, -1);
            EnsureTestUsersExist(stakeholdersContext);
            
            var controller = CreateController(scope, "-1");

            var dto = new CreateGroupTravelRequestDto
            {
                TourId = -1,
                ParticipantEmails = new List<string> { "turista1@example.com" }
            };

            var result = controller.Create(dto);
            var okResult = result.Result as OkObjectResult;
            var badRequestResult = result.Result as BadRequestObjectResult;
            
            if (badRequestResult != null)
            {
                throw new Exception($"Create failed: {badRequestResult.Value}");
            }

            okResult.ShouldNotBeNull();
            okResult.StatusCode.ShouldBe(200);
            var response = (GroupTravelRequestDto)okResult.Value;
            response.ShouldNotBeNull();
            response.TourId.ShouldBe(-1);
            response.OrganizerId.ShouldBe(-1);
            response.Participants.Count.ShouldBe(1);
            response.Status.ShouldBe(0);

            var savedRequest = context.GroupTravelRequests
                .Include(r => r.Participants)
                .FirstOrDefault(r => r.Id == response.Id);
            savedRequest.ShouldNotBeNull();
            savedRequest.Participants.Count.ShouldBe(1);
        }

        [Fact]
        public void GetMyRequests_returns_organizer_requests()
        {
            using var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var stakeholdersContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            
            EnsureTestTourExists(toursContext, -1);
            EnsureTestUsersExist(stakeholdersContext);
            
            var controller = CreateController(scope, "-1");

            var dto = new CreateGroupTravelRequestDto
            {
                TourId = -1,
                ParticipantEmails = new List<string> { "turista1@example.com" }
            };

            controller.Create(dto);

            var result = controller.GetMyRequests();
            var okResult = result.Result as OkObjectResult;

            okResult.StatusCode.ShouldBe(200);
            var requests = (List<GroupTravelRequestDto>)okResult.Value;
            requests.ShouldNotBeNull();
            requests.Count.ShouldBeGreaterThan(0);
            requests.Any(r => r.OrganizerId == -1).ShouldBeTrue();
        }

        [Fact]
        public void GetRequestsForMe_returns_participant_requests()
        {
            using var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var stakeholdersContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            
            EnsureTestTourExists(toursContext, -1);
            EnsureTestUsersExist(stakeholdersContext);
            
            var organizerController = CreateController(scope, "-1");
            var participantController = CreateController(scope, "-2");

            var dto = new CreateGroupTravelRequestDto
            {
                TourId = -1,
                ParticipantEmails = new List<string> { "turista1@example.com" }
            };

            organizerController.Create(dto);

            var result = participantController.GetRequestsForMe();
            var okResult = result.Result as OkObjectResult;

            okResult.StatusCode.ShouldBe(200);
            var requests = (List<GroupTravelRequestDto>)okResult.Value;
            requests.ShouldNotBeNull();
            requests.Count.ShouldBeGreaterThan(0);
            requests.Any(r => r.Participants.Any(p => p.TouristId == -2)).ShouldBeTrue();
        }

        [Fact]
        public void CompleteRequest_creates_tokens_for_all()
        {
            using var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            var toursContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var stakeholdersContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            
            EnsureTestTourExists(toursContext, -1);
            EnsureTestUsersExist(stakeholdersContext);
            
            var organizerController = CreateController(scope, "-1");
            var participantController = CreateController(scope, "-2");

            var organizerWallet = context.Wallets.FirstOrDefault(w => w.TouristId == -1);
            if (organizerWallet == null)
            {
                organizerWallet = new Wallet(-1);
                context.Wallets.Add(organizerWallet);
            }
            organizerWallet.AddBalance(1000m);
            context.SaveChanges();

            var participantWallet = context.Wallets.FirstOrDefault(w => w.TouristId == -2);
            if (participantWallet == null)
            {
                participantWallet = new Wallet(-2);
                context.Wallets.Add(participantWallet);
            }
            participantWallet.AddBalance(500m);
            context.SaveChanges();

            var createDto = new CreateGroupTravelRequestDto
            {
                TourId = -1,
                ParticipantEmails = new List<string> { "turista1@example.com" }
            };

            var createResult = organizerController.Create(createDto);
            var createOkResult = createResult.Result as OkObjectResult;
            var request = (GroupTravelRequestDto)createOkResult.Value;
            var requestId = request.Id;

            participantController.AcceptRequest(requestId).Wait();

            var completeResult = organizerController.CompleteRequest(requestId).Result;
            var completeOkResult = completeResult.Result as OkObjectResult;

            completeOkResult.StatusCode.ShouldBe(200);
            var tokens = (List<TourPurchaseTokenDto>)completeOkResult.Value;
            tokens.ShouldNotBeNull();
            tokens.Count.ShouldBe(2);
            tokens.Any(t => t.TouristId == -1).ShouldBeTrue();
            tokens.Any(t => t.TouristId == -2).ShouldBeTrue();

            var savedRequest = context.GroupTravelRequests.FirstOrDefault(r => r.Id == requestId);
            savedRequest.Status.ShouldBe(GroupTravelStatus.Completed);
        }

        private static void EnsureTestTourExists(ToursContext toursDb, int tourId)
        {
            if (!toursDb.Tours.Any(t => t.Id == tourId))
            {
                var tour = new Tour(tourId, $"Test Tour {tourId}", $"Description {tourId}", TourDifficulty.Easy, 
                    new List<string> { "test" }, TourStatus.Published, -11, 
                    new List<TourPoint>(), new List<Equipment>(), 100m, 
                    new List<TourTransportDuration>(), DateTime.UtcNow, null, 0.0);
                toursDb.Tours.Add(tour);
                toursDb.SaveChanges();
            }
        }

        private static void EnsureTestUsersExist(StakeholdersContext stakeholdersDb)
        {
            if (!stakeholdersDb.Users.Any(u => u.Id == -1))
            {
                var organizer = new Explorer.Stakeholders.Core.Domain.User(
                    "organizer",
                    "password",
                    Explorer.Stakeholders.Core.Domain.UserRole.Tourist,
                    true);
                stakeholdersDb.Users.Add(organizer);
                stakeholdersDb.Entry(organizer).Property("Id").CurrentValue = -1L;
                stakeholdersDb.SaveChanges();

                var organizerPerson = new Explorer.Stakeholders.Core.Domain.Person(-1L, "Organizer", "Test", "organizer@example.com");
                stakeholdersDb.People.Add(organizerPerson);
            }

            if (!stakeholdersDb.Users.Any(u => u.Id == -2))
            {
                var participant = new Explorer.Stakeholders.Core.Domain.User(
                    "turista1@example.com",
                    "password",
                    Explorer.Stakeholders.Core.Domain.UserRole.Tourist,
                    true);
                stakeholdersDb.Users.Add(participant);
                stakeholdersDb.Entry(participant).Property("Id").CurrentValue = -2L;
                stakeholdersDb.SaveChanges();

                var participantPerson = new Explorer.Stakeholders.Core.Domain.Person(-2L, "Turista", "One", "turista1@example.com");
                stakeholdersDb.People.Add(participantPerson);
            }

            if (!stakeholdersDb.Users.Any(u => u.Id == -3))
            {
                var participant2 = new Explorer.Stakeholders.Core.Domain.User(
                    "turista2@example.com",
                    "password",
                    Explorer.Stakeholders.Core.Domain.UserRole.Tourist,
                    true);
                stakeholdersDb.Users.Add(participant2);
                stakeholdersDb.Entry(participant2).Property("Id").CurrentValue = -3L;
                stakeholdersDb.SaveChanges();

                var participant2Person = new Explorer.Stakeholders.Core.Domain.Person(-3L, "Turista", "Two", "turista2@example.com");
                stakeholdersDb.People.Add(participant2Person);
            }

            stakeholdersDb.SaveChanges();
        }

        private GroupTravelController CreateController(IServiceScope scope, string userId)
        {
            return new GroupTravelController(
                scope.ServiceProvider.GetRequiredService<IGroupTravelService>(),
                scope.ServiceProvider.GetRequiredService<INotificationService>(),
                scope.ServiceProvider.GetRequiredService<IHubContext<MessageHub>>())
            {
                ControllerContext = BuildTouristContext(userId)
            };
        }
    }
}

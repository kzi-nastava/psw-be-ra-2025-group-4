using Explorer.API.Controllers.Administrator.Administration;
using Explorer.API.Controllers.Tourist.Encounters;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Administration;
using Explorer.Encounters.API.Public.Tourist;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class EncounterTouristTests : BaseEncountersIntegrationTest
    {
        public EncounterTouristTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_paged_encounters()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetActive().Result)?.Value as IEnumerable<EncounterDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(4);
        }

        [Fact]
        public void can_read_seeded_encounters()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = ((ObjectResult)controller.GetActive().Result)?.Value as IEnumerable<EncounterDto>;

            result.ShouldNotBeNull();
            result.Count().ShouldBeGreaterThanOrEqualTo(2);

            var first = result.FirstOrDefault(e => e.Id == -1);
            first.ShouldNotBeNull();
            first.Name.ShouldBe("Test Encounter One");
        }

        [Fact]
        public void can_activate_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.Activate(-1);

            // Assert
            result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public void social_encounter_completes_when_minimum_participants_met()
        {
            using var scope = Factory.Services.CreateScope();

            // Get the repository
            var executionRepository = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>();

            // Create multiple tourists
            var controller1 = CreateController(scope, userId: "-22");
            var controller2 = CreateController(scope, userId: "-23");

            // Both activate the social encounter
            controller1.Activate(-1);
            controller2.Activate(-1);

            var location = new LocationDto
            {
                Latitude = 45.2671,
                Longitude = 19.8335
            };

            // Both enter the radius
            controller1.UpdateTouristsLocationSocial(-1, location);
            controller2.UpdateTouristsLocationSocial(-1, location);

            // Assert - check both executions are completed
            var execution1 = executionRepository.Get(-22, -1);
            var execution2 = executionRepository.Get(-23, -1);

            execution1.ShouldNotBeNull();
            execution1.Status.ShouldBe(EncounterExecutionStatus.Completed);

            execution2.ShouldNotBeNull();
            execution2.Status.ShouldBe(EncounterExecutionStatus.Completed);
        }


        [Fact]
        public void get_by_tourist_returns_encounters_for_given_tourist()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristEncounterService>();

            // Act
            var result = service.GetByTourist(-21).ToList();

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void get_by_tour_point_returns_encounters()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristEncounterService>();

            var touristLocation = new LocationDto
            {
                Latitude = 45.2671,
                Longitude = 19.8335
            };

            // Act
            var result = service.GetByTourPoint(
                touristId: -21,
                tourPointId: -1,
                touristLocation: touristLocation);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType<List<EncounterViewDto>>();
            result.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void CompleteEncounter_Throws_When_EncounterNotFound()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristEncounterService>();

            Should.Throw<NotFoundException>(() =>
                service.CompleteEncounter(touristId: -21, encounterId: -999));
        }


        [Fact]
        public void CompleteEncounter_Throws_When_ExecutionNotFound()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristEncounterService>();

            Should.Throw<NotFoundException>(() =>
                service.CompleteEncounter(touristId: -21, encounterId: -1));
        }

        [Fact]
        public void CompleteEncounter_Throws_When_EncounterNotMisc()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristEncounterService>();

            var executionRepo = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>();
            executionRepo.Create(new EncounterExecution(-23, -5));

            Should.Throw<ArgumentException>(() =>
                service.CompleteEncounter(touristId: -23, encounterId: -5));
        }


        [Fact]
        public void CompleteEncounter_Throws_When_AlreadyCompleted()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristEncounterService>();

            var executionRepo = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>();
            var execution = new EncounterExecution(-21, -3);
            execution.Complete();
            executionRepo.Create(execution);

            Should.Throw<InvalidOperationException>(() =>
                service.CompleteEncounter(touristId: -21, encounterId: -3));
        }

        [Fact]
        public void CompleteEncounter_CompletesSuccessfully_When_ValidMiscEncounter()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITouristEncounterService>();

            var executionRepo = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>();
            var participantRepo = scope.ServiceProvider.GetRequiredService<IEncounterParticipantRepository>();

            executionRepo.Create(new EncounterExecution(-23, -4));

            var result = service.CompleteEncounter(-23, -4);

            result.ShouldNotBeNull();
            result.IsCompleted.ShouldBeTrue();
            result.ExperiencePointsGained.ShouldBe(150); 

            var participant = participantRepo.Get(-23);
            participant.ExperiencePoints.ShouldBeGreaterThanOrEqualTo(200);
        }

        [Fact]
        public void UpdateLocation_CompletesEncounter_When_Held_LongEnough()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var executionRepo = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>();
            var participantRepo = scope.ServiceProvider.GetRequiredService<IEncounterParticipantRepository>();

            executionRepo.ShouldNotBeNull();
            participantRepo.ShouldNotBeNull();

            var execution = new EncounterExecution(-21, -2);
            execution.SetWithinRadius(DateTime.UtcNow.AddSeconds(-32)); 
            executionRepo.Create(execution);

            var nearLocation = new LocationDto { Latitude = 44.7866, Longitude = 20.4489 }; 

            var actionResult = controller.UpdateTouristsLocationHidden(-2, nearLocation);
            var okResult = actionResult.Result as OkObjectResult; 
            okResult.ShouldNotBeNull();

            var dto = okResult.Value as EncounterUpdateResultDto;
            dto.ShouldNotBeNull();
            dto.IsCompleted.ShouldBeTrue();
            dto.ExperiencePointsGained.ShouldBeGreaterThan(0);

        }

        private EncounterExecution SeedExecution(IServiceScope scope, long touristId, long encounterId, DateTime? withinRadius = null)
        {
            var repo = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>();
            var execution = new EncounterExecution(touristId, encounterId);
            if (withinRadius.HasValue)
                execution.SetWithinRadius(withinRadius.Value);
            repo.Create(execution);
            return execution;
        }

        [Fact]
        public void EntersRadius_When_FirstTimeInside()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var execution = SeedExecution(scope, -21, -10);

            var nearLocation = new LocationDto { Latitude = 44.7866, Longitude = 20.4489 };
            var actionResult = controller.UpdateTouristsLocationHidden(-10, nearLocation);

            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var dto = okResult.Value as EncounterUpdateResultDto;
            dto.ShouldNotBeNull();
            dto.IsCompleted.ShouldBeFalse(); 

            var updatedExecution = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>()
                .Get(-21, -10);
            updatedExecution.WithinRadiusSinceUtc.ShouldNotBeNull();
        }


        [Fact]
        public void LeavesRadius_When_PreviouslyInside_ThenMovedOutside()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var execution = SeedExecution(scope, -21, -10, DateTime.UtcNow.AddSeconds(-5));

            var farLocation = new LocationDto { Latitude = 0.0, Longitude = 0.0 }; 
            var actionResult = controller.UpdateTouristsLocationHidden(-10, farLocation);

            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var dto = okResult.Value as EncounterUpdateResultDto;
            dto.ShouldNotBeNull();
            dto.IsCompleted.ShouldBeFalse(); 
        }

        [Fact]
        public void DoesNotComplete_If_NotHeldLongEnough()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var executionRepo = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>();

            var execution = SeedExecution(scope, -21, -10, DateTime.UtcNow.AddSeconds(-25));

            var nearLocation = new LocationDto { Latitude = 44.7866, Longitude = 20.4489 };

            var actionResult = controller.UpdateTouristsLocationHidden(-10, nearLocation);
            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var dto = okResult.Value as EncounterUpdateResultDto;
            dto.ShouldNotBeNull();
            dto.IsCompleted.ShouldBeFalse();
            dto.ExperiencePointsGained.ShouldBe(0);

            var updatedExecution = executionRepo.Get(-21, -10);
            updatedExecution.Status.ShouldBe(EncounterExecutionStatus.Started);
        }

        [Fact]
        public void Throws_When_EncounterDoesNotExist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var loc = new LocationDto { Latitude = 0, Longitude = 0 };

            Should.Throw<NotFoundException>(() =>
                controller.UpdateTouristsLocationHidden(-999, loc));
        }

        [Fact]
        public void Throws_When_ExecutionDoesNotExist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var loc = new LocationDto { Latitude = 44.7866, Longitude = 20.4489 };

            Should.Throw<InvalidOperationException>(() =>
                controller.UpdateTouristsLocationHidden(-11, loc));
        }

        [Fact]
        public void Throws_When_ExecutionAlreadyCompleted()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var executionRepo = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>();
            var execution = new EncounterExecution(-21, -6);
            execution.Complete();
            executionRepo.Create(execution);

            var loc = new LocationDto { Latitude = 44.7866, Longitude = 20.4489 };

            Should.Throw<ArgumentException>(() =>
                controller.UpdateTouristsLocationHidden(-6, loc));
        }

        [Fact]
        public void GetByTourPoint_Returns_List_For_Valid_TourPoint()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            int tourPointId = -1;
            double lat = 44.7866;
            double lon = 20.4489;

            var result = controller.GetByTourPoint(tourPointId, lat, lon).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var encounters = result.Value as List<EncounterViewDto>;
            encounters.ShouldNotBeNull();
            encounters.Count.ShouldBeGreaterThan(0);

            var first = encounters.First();
            first.ShouldNotBeNull();
            first.Id.ShouldNotBe(0);
            first.Name.ShouldNotBeNullOrEmpty();
        }

        [Fact]
        public void GetByTourPoint_Returns_Empty_For_TourPoint_With_No_Encounters()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            int tourPointId = -999;
            double lat = 44.7866;
            double lon = 20.4489;

            var result = controller.GetByTourPoint(tourPointId, lat, lon).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var encounters = result.Value as List<EncounterViewDto>;
            encounters.ShouldNotBeNull();
            encounters.ShouldBeEmpty();
        }

        [Fact]
        public void SubmitQuizAnswer_Completes_When_Correct_And_Within_Time()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, userId: "-90");

            var executionRepo = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>();
            var participantRepo = scope.ServiceProvider.GetRequiredService<IEncounterParticipantRepository>();

            // Quiz encounter id assumed seeded
            long encounterId = -6;
            long touristId = -90;

            // Activate quiz encounter
            var ex = new EncounterExecution(touristId, encounterId);
            ex.SetStartedAt(DateTime.UtcNow.AddSeconds(-10));
            executionRepo.Create(ex);


            var answerDto = new QuizAnswerSubmitDto
            {
                QuestionId = -70,
                SelectedAnswerId = -700 // correct answer
            };

            // Act
            var actionResult = controller.SubmitQuizAnswer(encounterId, new List<QuizAnswerSubmitDto> { answerDto });
            var okResult = actionResult.Result as OkObjectResult;

            // Assert
            okResult.ShouldNotBeNull();
            var result = okResult.Value as EncounterUpdateResultDto;

            result.ShouldNotBeNull();
            result.IsCompleted.ShouldBeTrue();
            result.ExperiencePointsGained.ShouldBeGreaterThan(0);

            var execution = executionRepo.Get(touristId, encounterId);
            execution.Status.ShouldBe(EncounterExecutionStatus.Completed);

            participantRepo.Get(touristId)
                .ExperiencePoints.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void SubmitQuizAnswer_Throws_When_AlreadyCompleted()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, userId: "-99");

            long encounterId = -6;
            long touristId = -99;

            var execution = new EncounterExecution(touristId, encounterId);
            execution.Complete();

            scope.ServiceProvider
                .GetRequiredService<IEncounterExecutionRepository>()
                .Create(execution);

            var answerDto = new QuizAnswerSubmitDto
            {
                QuestionId = -70,
                SelectedAnswerId = -700
            };

            Should.Throw<InvalidOperationException>(() =>
                controller.SubmitQuizAnswer(encounterId, new List<QuizAnswerSubmitDto> { answerDto }));
        }

        [Fact]
        public void SubmitQuizAnswer_Throws_When_ExecutionNotFound()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, userId: "-98");

            var answerDto = new QuizAnswerSubmitDto
            {
                QuestionId = -70,
                SelectedAnswerId = -700
            };

            Should.Throw<NotFoundException>(() =>
                controller.SubmitQuizAnswer(-6, new List<QuizAnswerSubmitDto> { answerDto }));
        }

        [Fact]
        public void SubmitQuizAnswer_Throws_When_TimeLimitExceeded()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, userId: "-97");

            long encounterId = -6;
            long touristId = -97;

            var executionRepo = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>();
            var ex = new EncounterExecution(touristId, encounterId);
            ex.SetStartedAt(DateTime.UtcNow.AddSeconds(-1000));
            executionRepo.Create(ex);

            var answerDto = new QuizAnswerSubmitDto
            {
                QuestionId = -70,
                SelectedAnswerId = -700
            };

            Should.Throw<InvalidOperationException>(() =>
                controller.SubmitQuizAnswer(encounterId, new List<QuizAnswerSubmitDto> { answerDto }));
        }

        [Fact]
        public void SubmitQuizAnswer_DoesNotComplete_When_AnswerIsWrong()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, userId: "-95");

            long encounterId = -6;
            long touristId = -95;

            var executionRepo = scope.ServiceProvider.GetRequiredService<IEncounterExecutionRepository>();
            var ex = new EncounterExecution(touristId, encounterId);
            ex.SetStartedAt(DateTime.UtcNow.AddSeconds(-10));
            executionRepo.Create(ex);

            var answerDto = new QuizAnswerSubmitDto
            {
                QuestionId = -70,
                SelectedAnswerId = -701 
            };

            var result = controller.SubmitQuizAnswer(encounterId, new List<QuizAnswerSubmitDto> { answerDto })
                .Result as OkObjectResult;

            result.ShouldNotBeNull();

            var dto = result.Value as EncounterUpdateResultDto;
            dto.ShouldNotBeNull();
            dto.IsCompleted.ShouldBeFalse();
            dto.ExperiencePointsGained.ShouldBe(0);

            executionRepo.Get(touristId, encounterId)
                .Status.ShouldBe(EncounterExecutionStatus.Completed);
        }




        private static TouristEncountersController CreateController(IServiceScope scope, string userId = "-21")
        {
            return new TouristEncountersController(
                scope.ServiceProvider.GetRequiredService<IEncounterService>(),
                scope.ServiceProvider.GetRequiredService<ITouristEncounterService>(),
                scope.ServiceProvider.GetRequiredService<IEncounterParticipantService>())
            {
                ControllerContext = BuildContext(userId)
            };
        }
    }
}

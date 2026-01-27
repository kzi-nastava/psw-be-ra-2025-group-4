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
        public void Creating_social_encounter_below_level_throws()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, userId: "-22"); //-22 je level 6, pa ne moze

            var dto = new SocialEncounterDto
            {
                Name = "Test Encounter",
                Description = "Desc",
                MinimumParticipants = 2,
                ActivationRadiusMeters = 50,
                Location = new LocationDto { Latitude = 45, Longitude = 19 },
                ExperiencePoints = 50
            };

            Should.Throw<InvalidOperationException>(() => controller.CreateSocial(dto));
        }

        [Fact]
        public void can_update_social_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new SocialEncounterDto
            {
                Id = -1,
                Name = "Updated Social Encounter",
                Description = "Updated description",
                MinimumParticipants = 3,
                ActivationRadiusMeters = 150,
                Location = new LocationDto { Latitude = 45.2671, Longitude = 19.8335 },
                ExperiencePoints = 120
            };

            var actionResult = controller.UpdateSocial(-1, dto);
            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var updated = okResult.Value as SocialEncounterDto;
            updated.ShouldNotBeNull();
            updated.Name.ShouldBe("Updated Social Encounter");
            updated.ActivationRadiusMeters.ShouldBe(150);
        }

        [Fact]
        public void can_update_hidden_location_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new HiddenLocationEncounterDto
            {
                Id = -2,
                Name = "Updated Hidden Encounter",
                Description = "Updated hidden description",
                ExperiencePoints = 250,
                ImageUrl = "http://example.com/updated.png",
                ActivationRadiusMeters = 30,
                Location = new LocationDto { Latitude = 45.8150, Longitude = 15.9819 },
                PhotoPoint = new LocationDto { Latitude = 45.8160, Longitude = 15.9820 }
            };

            // Act
            var actionResult = controller.UpdateHiddenLocation(-2, dto);

            // Cast to OkObjectResult to get the value
            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var updated = okResult.Value as HiddenLocationEncounterDto;
            updated.ShouldNotBeNull();
            updated.Name.ShouldBe("Updated Hidden Encounter");
            updated.ActivationRadiusMeters.ShouldBe(30);
        }


        [Fact]
        public void can_delete_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            controller.Delete(-3);

            var getResult = ((ObjectResult)controller.GetActive().Result)?.Value as IEnumerable<EncounterDto>;
            getResult.ShouldNotContain(e => e.Id == -3);
        }

        [Fact]
        public void can_create_social_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new SocialEncounterDto
            {
                Name = "New Social Encounter",
                Description = "Created via test",
                MinimumParticipants = 2,
                ActivationRadiusMeters = 100,
                Location = new LocationDto { Latitude = 45.2500, Longitude = 19.8335 },
                ExperiencePoints = 50
            };

            // Act
            var actionResult = controller.CreateSocial(dto);
            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var created = okResult.Value as SocialEncounterDto;
            created.ShouldNotBeNull();
            created.Name.ShouldBe("New Social Encounter");
            created.ActivationRadiusMeters.ShouldBe(100);
        }

        [Fact]
        public void can_create_hidden_location_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new HiddenLocationEncounterDto
            {
                Name = "New Hidden Encounter",
                Description = "Created via test",
                ExperiencePoints = 200,
                ImageUrl = "http://example.com/new.png",
                ActivationRadiusMeters = 500,
                Location = new LocationDto { Latitude = 45.8000, Longitude = 15.9800 },
                PhotoPoint = new LocationDto { Latitude = 45.8010, Longitude = 15.9810 }
            };

            // Act
            var actionResult = controller.CreateHiddenLocation(dto);
            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var created = okResult.Value as HiddenLocationEncounterDto;
            created.ShouldNotBeNull();    
            created.Name.ShouldBe("New Hidden Encounter");
            created.ActivationRadiusMeters.ShouldBe(500);
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
        public void can_create_quiz_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new QuizEncounterDto
            {
                Name = "Quiz Encounter",
                Description = "Quiz description",
                ExperiencePoints = 100,
                TimeLimit = 60,
                Location = new LocationDto
                {
                    Latitude = 45.2671,
                    Longitude = 19.8335
                },
                Questions = new List<QuizQuestionDto>
        {
            new QuizQuestionDto
            {
                Text = "What is 2 + 2?",
                Answers = new List<QuizAnswerDto>
                {
                    new QuizAnswerDto { Text = "4", IsCorrect = true },
                    new QuizAnswerDto { Text = "5", IsCorrect = false }
                }
            }
        }
            };

            // Act
            var actionResult = controller.CreateQuiz(dto);
            var okResult = actionResult.Result as OkObjectResult;

            // Assert
            okResult.ShouldNotBeNull();

            var created = okResult.Value as QuizEncounterDto;
            created.ShouldNotBeNull();
            created.Name.ShouldBe("Quiz Encounter");
            created.TimeLimit.ShouldBe(60);
            created.Questions.Count.ShouldBe(1);
            created.Questions.First().Answers.Any(a => a.IsCorrect).ShouldBeTrue();
        }

        [Fact]
        public void can_update_quiz_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new QuizEncounterDto
            {
                Id = -6,
                Name = "Updated Quiz",
                Description = "Updated description",
                ExperiencePoints = 200,
                TimeLimit = 120,
                Location = new LocationDto
                {
                    Latitude = 45.3000,
                    Longitude = 19.8000
                },
                Questions = new List<QuizQuestionDto>
        {
            new QuizQuestionDto
            {
                Text = "Updated question?",
                Answers = new List<QuizAnswerDto>
                {
                    new QuizAnswerDto { Text = "Yes", IsCorrect = true },
                    new QuizAnswerDto { Text = "No", IsCorrect = false }
                }
            }
        }
            };

            // Act
            var actionResult = controller.UpdateQuiz(dto, -6);
            var okResult = actionResult.Result as OkObjectResult;

            // Assert
            okResult.ShouldNotBeNull();

            var updated = okResult.Value as QuizEncounterDto;
            updated.ShouldNotBeNull();
            updated.Name.ShouldBe("Updated Quiz");
            updated.TimeLimit.ShouldBe(120);
            updated.Questions.Single().Text.ShouldBe("Updated question?");
        }

        [Fact]
        public void create_quiz_fails_when_no_correct_answer()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new QuizEncounterDto
            {
                Name = "Invalid Quiz",
                Description = "No correct answers",
                ExperiencePoints = 50,
                TimeLimit = 30,
                Location = new LocationDto { Latitude = 45, Longitude = 19 },
                Questions = new List<QuizQuestionDto>
        {
            new QuizQuestionDto
            {
                Text = "Invalid?",
                Answers = new List<QuizAnswerDto>
                {
                    new QuizAnswerDto { Text = "A", IsCorrect = false },
                    new QuizAnswerDto { Text = "B", IsCorrect = false }
                }
            }
        }
            };

            Should.Throw<ArgumentException>(() => controller.CreateQuiz(dto));
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

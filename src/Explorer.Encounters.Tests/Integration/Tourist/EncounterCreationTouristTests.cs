using Explorer.API.Controllers.Tourist.Encounters;
using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Administration;
using Explorer.Encounters.API.Public.Tourist;
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
    public class EncounterCreationTouristTests : BaseEncountersIntegrationTest
    {

        public EncounterCreationTouristTests(EncountersTestFactory factory) : base(factory) { }
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
                        Answers = new List<EncounterQuizAnswerDto>
                        {
                            new EncounterQuizAnswerDto { Text = "4", IsCorrect = true },
                            new EncounterQuizAnswerDto { Text = "5", IsCorrect = false }
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
                        Answers = new List<EncounterQuizAnswerDto>
                        {
                            new EncounterQuizAnswerDto { Text = "Yes", IsCorrect = true },
                            new EncounterQuizAnswerDto { Text = "No", IsCorrect = false }
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
                        Answers = new List<EncounterQuizAnswerDto>
                        {
                            new EncounterQuizAnswerDto { Text = "A", IsCorrect = false },
                            new EncounterQuizAnswerDto { Text = "B", IsCorrect = false }
                        }
                    }
                }
            };

            Should.Throw<ArgumentException>(() => controller.CreateQuiz(dto));
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
                PhotoPoint = new LocationDto { Latitude = 44.7866, Longitude = 20.4489 }
            };

            // Act
            var actionResult = controller.UpdateHiddenLocation(-2, dto);



            // Cast to OkObjectResult to get the value
            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var updated = okResult.Value as HiddenLocationEncounterDto;
            updated.ShouldNotBeNull();
            updated.Name.ShouldBe("Updated Hidden Encounter");
            updated.Description.ShouldBe("Updated hidden description");
            updated.ExperiencePoints.ShouldBe(250);
            updated.ImageUrl.ShouldBe("http://example.com/updated.png");
            updated.ActivationRadiusMeters.ShouldBe(30);
            updated.PhotoPoint.Latitude.ShouldBe(44.7866);
            updated.PhotoPoint.Longitude.ShouldBe(20.4489);
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
        public void CreateMisc_Throws_If_Tourist_Level_Too_Low()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, userId: "-22"); // Assume -22 is below level 10

            var dto = new EncounterDto
            {
                Name = "Misc Test",
                Description = "Level check test",
                ExperiencePoints = 50,
                Type = EncounterType.Misc,
                Location = new LocationDto { Latitude = 45, Longitude = 19 }
            };

            Should.Throw<InvalidOperationException>(() =>
            {
                controller.CreateMisc(dto);
            }).Message.ShouldBe("You have to be atleast level 10 to create encounters!");
        }

        [Fact]
        public void CreateMisc_Succeeds_For_Valid_Level()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, userId: "-21"); // Assume -21 has level >= 10

            var dto = new EncounterDto
            {
                Name = "Misc Valid",
                Description = "Valid creation test",
                ExperiencePoints = 100,
                Type = EncounterType.Misc,
                Location = new LocationDto { Latitude = 45.1, Longitude = 19.1 }
            };

            var result = controller.CreateMisc(dto).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var created = result.Value as EncounterDto;
            created.ShouldNotBeNull();
            created.Name.ShouldBe("Misc Valid");
            created.ExperiencePoints.ShouldBe(100);
            created.Type.ShouldBe(EncounterType.Misc);
        }

        [Fact]
        public void UpdateMisc_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, userId: "-21");

            var dto = new EncounterUpdateDto
            {
                Id = -5,
                Name = "Updated Misc",
                Description = "Updated description",
                ExperiencePoints = 200,
                Type = EncounterType.Misc,
                Location = new LocationDto { Latitude = 45.2, Longitude = 19.2 }
            };

            var result = controller.UpdateMisc(-5, dto).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var updated = result.Value as EncounterDto;
            updated.ShouldNotBeNull();
            updated.Id.ShouldBe(-5);
            updated.Name.ShouldBe("Updated Misc");
            updated.ExperiencePoints.ShouldBe(200);
        }

        [Fact]
        public void Service_Create_Creates_Encounter_With_Approval_When_NeedsApproval_True()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var dto = new EncounterDto
            {
                Name = "Service Misc Test",
                Description = "Testing service create",
                ExperiencePoints = 50,
                Type = EncounterType.Misc,
                Location = new LocationDto { Latitude = 45.3, Longitude = 19.3 }
            };

            var created = service.Create(dto, needsApproval: true);
            created.ShouldNotBeNull();
            created.Name.ShouldBe("Service Misc Test");
            created.Type.ShouldBe(EncounterType.Misc);
        }

        [Fact]
        public void Service_Update_Updates_Encounter_Correctly()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var dto = new EncounterUpdateDto
            {
                Id = -6,
                Name = "Service Update Misc",
                Description = "Update via service",
                ExperiencePoints = 150,
                Type = EncounterType.Misc,
                Location = new LocationDto { Latitude = 45.4, Longitude = 19.4 }
            };

            var updated = service.Update(dto, -6);
            updated.ShouldNotBeNull();
            updated.Name.ShouldBe("Service Update Misc");
            updated.ExperiencePoints.ShouldBe(150);
        }
    }
}

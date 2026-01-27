using Explorer.API.Controllers.Author;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Administration;
using Explorer.Tours.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Encounters.Tests.Integration.Author
{
    [Collection("Sequential")]
    public class EncounterAuthorTests : BaseEncountersIntegrationTest
    {
        public EncounterAuthorTests(EncountersTestFactory factory) : base(factory) { }

        private static EncounterController CreateController(IServiceScope scope, string userId = "-11")
        {
            return new EncounterController(
                scope.ServiceProvider.GetRequiredService<IEncounterService>(),
                scope.ServiceProvider.GetRequiredService<ITourService>())
            {
                ControllerContext = BuildContext(userId)
            };
        }

        [Fact]
        public void Can_Get_Paged_Encounters()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = ((ObjectResult)controller.GetPaged(0, 10).Result)?.Value as PagedResult<EncounterViewDto>;

            result.ShouldNotBeNull();
            result.Results.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Can_Get_Encounters_By_TourId()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();
            var tour = tourService.GetById(-1); 
            tour.ShouldNotBeNull();
            tour.Name.ShouldBe("Test Tura 1");

            var result = controller.GetByTourId(-1).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var encounters = result.Value as List<EncounterViewDto>;
            encounters.ShouldNotBeNull();
            encounters.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Can_Create_Social_Encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new SocialEncounterDto
            {
                Name = "Author Social Test",
                Description = "Created by author test",
                MinimumParticipants = 2,
                ActivationRadiusMeters = 50,
                Location = new LocationDto { Latitude = 45, Longitude = 19 },
                ExperiencePoints = 100
            };

            var result = controller.CreateSocial(dto).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var created = result.Value as SocialEncounterDto;
            created.ShouldNotBeNull();
            created.Name.ShouldBe("Author Social Test");
            created.ActivationRadiusMeters.ShouldBe(50);
        }

        [Fact]
        public void Can_Update_Social_Encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new SocialEncounterDto
            {
                Id = -1,
                Name = "Updated Author Social",
                Description = "Updated description",
                MinimumParticipants = 3,
                ActivationRadiusMeters = 150,
                Location = new LocationDto { Latitude = 45.1, Longitude = 19.1 },
                ExperiencePoints = 200
            };

            var result = controller.UpdateSocial(dto, -1).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var updated = result.Value as SocialEncounterDto;
            updated.ShouldNotBeNull();
            updated.Name.ShouldBe("Updated Author Social");
            updated.ActivationRadiusMeters.ShouldBe(150);
        }

        [Fact]
        public void Can_Create_HiddenLocation_Encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new HiddenLocationEncounterDto
            {
                Name = "Hidden Author Test",
                Description = "Hidden encounter by author",
                ExperiencePoints = 150,
                ImageUrl = "http://example.com/test.png",
                ActivationRadiusMeters = 30,
                Location = new LocationDto { Latitude = 45, Longitude = 19 },
                PhotoPoint = new LocationDto { Latitude = 45.001, Longitude = 19.001 }
            };

            var result = controller.CreateHidden(dto).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var created = result.Value as HiddenLocationEncounterDto;
            created.ShouldNotBeNull();
            created.Name.ShouldBe("Hidden Author Test");
            created.ActivationRadiusMeters.ShouldBe(30);
        }

        [Fact]
        public void Can_Update_HiddenLocation_Encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new HiddenLocationEncounterDto
            {
                Id = -2,
                Name = "Updated Hidden Author",
                Description = "Updated hidden description",
                ExperiencePoints = 250,
                ImageUrl = "http://example.com/updated.png",
                ActivationRadiusMeters = 40,
                Location = new LocationDto { Latitude = 45.2, Longitude = 19.2 },
                PhotoPoint = new LocationDto { Latitude = 45.201, Longitude = 19.201 }
            };

            var result = controller.UpdateHidden(dto, -2).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var updated = result.Value as HiddenLocationEncounterDto;
            updated.ShouldNotBeNull();
            updated.Name.ShouldBe("Updated Hidden Author");
            updated.ActivationRadiusMeters.ShouldBe(40);
        }

        [Fact]
        public void Can_Create_Quiz_Encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new QuizEncounterDto
            {
                Name = "Author Quiz",
                Description = "Quiz created by author",
                ExperiencePoints = 100,
                TimeLimit = 60,
                Location = new LocationDto { Latitude = 45.3, Longitude = 19.3 },
                Questions = new List<QuizQuestionDto>
                {
                    new QuizQuestionDto
                    {
                        Text = "What is 1+1?",
                        Answers = new List<QuizAnswerDto>
                        {
                            new QuizAnswerDto { Text = "2", IsCorrect = true },
                            new QuizAnswerDto { Text = "3", IsCorrect = false }
                        }
                    }
                }
            };

            var result = controller.CreateQuiz(dto).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var created = result.Value as QuizEncounterDto;
            created.ShouldNotBeNull();
            created.Name.ShouldBe("Author Quiz");
            created.Questions.Count.ShouldBe(1);
            created.Questions.First().Answers.Any(a => a.IsCorrect).ShouldBeTrue();
        }

        [Fact]
        public void Can_Update_Quiz_Encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new QuizEncounterDto
            {
                Id = -6,
                Name = "Updated Author Quiz",
                Description = "Updated quiz",
                ExperiencePoints = 200,
                TimeLimit = 120,
                Location = new LocationDto { Latitude = 45.4, Longitude = 19.4 },
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

            var result = controller.UpdateQuiz(dto, -6).Result as OkObjectResult;
            result.ShouldNotBeNull();

            var updated = result.Value as QuizEncounterDto;
            updated.ShouldNotBeNull();
            updated.Name.ShouldBe("Updated Author Quiz");
            updated.TimeLimit.ShouldBe(120);
        }

        [Fact]
        public void Can_Publish_Encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = controller.Publish(-7);
            result.ShouldBeOfType<NoContentResult>();
        }

        [Fact]
        public void Can_Archive_Encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = controller.Archive(-1);
            result.ShouldBeOfType<NoContentResult>();
        }

        [Fact]
        public void Can_Delete_Encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            controller.Delete(-4);

            var encounters = controller.GetPaged().Result as OkObjectResult;
            var paged = encounters.Value as PagedResult<EncounterViewDto>;
            paged.Results.ShouldNotContain(e => e.Id == -4);
        }
    }
}

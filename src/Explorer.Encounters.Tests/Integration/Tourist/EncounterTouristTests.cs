using Explorer.API.Controllers.Administrator.Administration;
using Explorer.API.Controllers.Tourist.Encounters;
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
            result.Count().ShouldBe(2);
        }

        [Fact]
        public void UpdateLocation_RemovesParticipantWhenOutOfRange()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            controller.Activate(-1);

            var dto = new TouristLocationDto
            {
                Latitude = 1000.0, 
                Longitude = 1000.0
            };

            var result = controller.UpdateSocialLocation(-1, dto) as ActionResult<int>;
            var activeCount = result?.Value;

            activeCount.ShouldBe(0);
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
                ActivationRadiusMeters = 50,
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
            created.ActivationRadiusMeters.ShouldBe(50);
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

using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Explorer.Encounters.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class EncounterAdminTests : BaseEncountersIntegrationTest
    {
        public EncounterAdminTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_paged_encounters()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetPaged(0, 20).Result)?.Value as PagedResult<EncounterViewDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Results.Count.ShouldBe(11);
        }

        [Fact]
        public void Creates_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new EncounterDto
            {
                Name = "New Encounter",
                Description = "Test encounter",
                ExperiencePoints = 100,
                Type = 0,
                Location = new LocationDto
                {
                    Latitude = 45.0,
                    Longitude = 19.0
                }
            };

            var actionResult = controller.Create(dto);
            var objectResult = actionResult.Result as ObjectResult;

            objectResult.ShouldNotBeNull();

            var created = objectResult.Value as EncounterDto;
            created.ShouldNotBeNull();
            created!.Name.ShouldBe("New Encounter");

        }

        [Fact]
        public void Updates_existing_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var update = new EncounterUpdateDto
            {
                Id = -1,
                Name = "Updated Encounter",
                Description = "Updated description",
                ExperiencePoints = 250,
                Type = EncounterType.Social,
                Location = new LocationDto
                {
                    Latitude = 44.5,
                    Longitude = 20.5
                }
            };

            var actionResult = controller.Update(update, (int)update.Id);

            var objectResult = actionResult.Result as ObjectResult;
            objectResult.ShouldNotBeNull();

            var updated = objectResult.Value as EncounterDto;
            updated.ShouldNotBeNull();
            updated.Name.ShouldBe("Updated Encounter");
        }

        [Fact]
        public void Deletes_encounter()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var response = controller.Delete(-2);

            response.ShouldBeOfType<NoContentResult>();
        }

        [Fact]
        public void GetPendingApproval_Returns_something()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = controller.GetPendingApproval();

            result.ShouldNotBeNull();
        }


        [Fact]
        public void Archive_DoesNotThrow()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var ex = Record.Exception(() => service.Archive(-6));
            Assert.Null(ex);
        }

        [Fact]
        public void Approve_DoesNotThrow()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var ex = Record.Exception(() => service.Approve(-5));
            Assert.Null(ex);
        }

        [Fact]
        public void Publish_DoesNotThrow()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var ex = Record.Exception(() => service.Publish(-3));
            Assert.Null(ex);
        }

        [Fact]
        public void Decline_DoesNotThrow()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var ex = Record.Exception(() => service.Decline(-6));
            Assert.Null(ex);
        }

        [Fact]
        public void AddEncounterToTourPoint_DoesNotThrow()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IEncounterService>();

            var encounterId = -6;
            var tourPointId = 999;
            var isRequired = true;

            service.AddEncounterToTourPoint(encounterId, tourPointId, isRequired);
        }



        private static EncounterController CreateController(IServiceScope scope)
        {
            return new EncounterController(
                scope.ServiceProvider.GetRequiredService<IEncounterService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}

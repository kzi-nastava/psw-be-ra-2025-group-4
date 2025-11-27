using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class FacilityCommandTests : BaseToursIntegrationTest
    {
        public FacilityCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var newEntity = new FacilityDto
            {
                Name = "Fontana Trg",
                Latitude = 44.8123,
                Longitude = 20.4611,
                Category = "Other"
            };

            // Act
            var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as FacilityDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Name.ShouldBe(newEntity.Name);

            // Assert - Database
            var storedEntity = dbContext.Facility.FirstOrDefault(i => i.Name == newEntity.Name);
            storedEntity.ShouldNotBeNull();
            storedEntity.Id.ShouldBe(result.Id);
        }

        [Fact]
        public void Create_fails_invalid_data()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var invalid = new FacilityDto
            {
                Latitude = 999,
                Longitude = 999,
                Category = "WC"
            };

            // Act & Assert
            Should.Throw<ArgumentException>(() => controller.Create(invalid));
        }

        [Fact]
        public void Updates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var updatedEntity = new FacilityDto
            {
                Id = -1,
                Name = "WC Renoviran",
                Latitude = 44.7866,
                Longitude = 20.4489,
                Category = "WC"
            };

            // Act
            var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as FacilityDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(-1);
            result.Name.ShouldBe(updatedEntity.Name);

            // Assert - Database
            var storedEntity = dbContext.Facility.FirstOrDefault(i => i.Name == "WC Renoviran");
            storedEntity.ShouldNotBeNull();

            var oldEntity = dbContext.Facility.FirstOrDefault(i => i.Name == "Javni WC");
            oldEntity.ShouldBeNull();
        }

        [Fact]
        public void Update_fails_invalid_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var invalid = new FacilityDto
            {
                Id = -9999,
                Name = "Test",
                Latitude = 44.0,
                Longitude = 20.0,
                Category = "Other"
            };

            // Act & Assert
            Should.Throw<NotFoundException>(() => controller.Update(invalid));
        }

        [Fact]
        public void Deletes()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Act
            var result = (OkResult)controller.Delete(-3);

            // Assert - Response
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Assert - Database
            var stored = dbContext.Facility.FirstOrDefault(i => i.Id == -3);
            stored.ShouldBeNull();
        }

        [Fact]
        public void Delete_fails_invalid_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            Should.Throw<NotFoundException>(() => controller.Delete(-9999));
        }

        private static FacilityController CreateController(IServiceScope scope)
        {
            return new FacilityController(scope.ServiceProvider.GetRequiredService<IFacilityService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}

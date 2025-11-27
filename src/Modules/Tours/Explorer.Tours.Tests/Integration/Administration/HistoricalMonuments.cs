using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Administration
{
    public class HistoricalMonumentsIntegrationTests : BaseToursIntegrationTest
    {
        private const string BaseUrl = "/api/administration/historical-monuments";

        public HistoricalMonumentsIntegrationTests(ToursTestFactory factory) : base(factory)
        {
        }
        [Fact]
        public void GetPaged_ShouldReturnSeededHistoricalMonuments()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IHistoricalMonumentService>();

            // Act
            var result = service.GetPaged(1, 10);

            // Assert
            result.Should().NotBeNull();
            result.Results.Should().NotBeNull();
            result.Results.Should().NotBeEmpty();

            foreach (var monument in result.Results)
            {
                monument.Name.Should().NotBeNullOrEmpty();
                monument.Description.Should().NotBeNullOrEmpty();
                monument.YearOfCreation.Should().BeGreaterThan(0);
                monument.Latitude.Should().BeInRange(-90, 90);
                monument.Longitude.Should().BeInRange(-180, 180);
            }

            result.Results.Should().Contain(m =>
                m.Id == -1 &&
                m.Name == "Test spomenik" &&
                m.Description == "Test description" &&
                m.YearOfCreation == 1900 &&
                m.AdministratorId == 1 &&
                m.Status == MonumentStatusDTO.Active
            );
        }

        [Fact]
        public void Create_ShouldCreateHistoricalMonument()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IHistoricalMonumentService>();

            var dto = new HistoricalMonumentDTO
            {
                Name = "Test Monument",
                Description = "Test description",
                YearOfCreation = 1980,
                Status = MonumentStatusDTO.Active,
                Latitude = 45.25,
                Longitude = 19.85,
                AdministratorId = 1
            };

            // Act
            var created = service.Create(dto);

            // Assert
            created.Should().NotBeNull();
            created.Id.Should().BeGreaterThan(0);
            created.Name.Should().Be(dto.Name);
            created.Description.Should().Be(dto.Description);
            created.YearOfCreation.Should().Be(1980);
            created.Status.Should().Be(MonumentStatusDTO.Active);
        }

        [Fact]
        public void Update_ShouldUpdateHistoricalMonument()
        {
            using var scopeCreate = Factory.Services.CreateScope();
            var serviceCreate = scopeCreate.ServiceProvider.GetRequiredService<IHistoricalMonumentService>();

            var createDto = new HistoricalMonumentDTO
            {
                Name = "ToUpdate",
                Description = "Before update",
                YearOfCreation = 1900,
                Status = MonumentStatusDTO.Active,
                Latitude = 45.0,
                Longitude = 19.8,
                AdministratorId = 1
            };

            var created = serviceCreate.Create(createDto);
            created.Should().NotBeNull();
            var id = created.Id;

            using var scopeUpdate = Factory.Services.CreateScope();
            var serviceUpdate = scopeUpdate.ServiceProvider.GetRequiredService<IHistoricalMonumentService>();

            var updateDto = new HistoricalMonumentDTO
            {
                Id = id,
                Name = "After update",
                Description = "Updated description",
                YearOfCreation = 1950,
                Status = MonumentStatusDTO.Inactive,
                Latitude = 45.1,
                Longitude = 19.9,
                AdministratorId = 1
            };

            var updated = serviceUpdate.Update(updateDto);

            updated.Should().NotBeNull();
            updated.Id.Should().Be(id);
            updated.Name.Should().Be("After update");
            updated.Description.Should().Be("Updated description");
            updated.Status.Should().Be(MonumentStatusDTO.Inactive);
            updated.YearOfCreation.Should().Be(1950);
        }

        [Fact]
        public void Delete_ShouldRemoveHistoricalMonument()
        {
            using var scopeCreate = Factory.Services.CreateScope();
            var serviceCreate = scopeCreate.ServiceProvider.GetRequiredService<IHistoricalMonumentService>();

            var createDto = new HistoricalMonumentDTO
            {
                Name = "ToDelete",
                Description = "To be deleted",
                YearOfCreation = 1920,
                Status = MonumentStatusDTO.Active,
                Latitude = 45.0,
                Longitude = 19.8,
                AdministratorId = 1
            };

            var created = serviceCreate.Create(createDto);
            created.Should().NotBeNull();
            var id = created.Id;

            using var scopeDelete = Factory.Services.CreateScope();
            var serviceDelete = scopeDelete.ServiceProvider.GetRequiredService<IHistoricalMonumentService>();

            serviceDelete.Delete(id);

            using var scopeCheck = Factory.Services.CreateScope();
            var serviceCheck = scopeCheck.ServiceProvider.GetRequiredService<IHistoricalMonumentService>();

            var all = serviceCheck.GetPaged(1, 100);
            all.Results.Should().NotContain(m => m.Id == id);
        }

    }

}

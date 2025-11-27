using System.Net;
using System.Net.Http.Json;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using FluentAssertions;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Administration
{
    public class HistoricalMonumentsIntegrationTests : BaseToursIntegrationTest
    {
        private const string BaseUrl = "/api/administration/historical-monuments";

        public HistoricalMonumentsIntegrationTests(ToursTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Get_all_historical_monuments_returns_ok()
        {
            // Act
            var response = await Client.GetAsync($"{BaseUrl}?page=1&pageSize=10");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result =
                await response.Content.ReadFromJsonAsync<PagedResult<HistoricalMonumentDTO>>();

            result.Should().NotBeNull();
            result!.Results.Should().NotBeNull();
            // ako imaš seed u b-insert-historical-monuments.sql možeš dodati:
            // result.Results.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Create_historical_monument_returns_ok()
        {
            var dto = new HistoricalMonumentDTO
            {
                Name = "Test Monument",
                Description = "Test description",
                YearOfCreation = 1980,
                Status = MonumentStatusDTO.Active,
                Latitude = 45.25,
                Longitude = 19.85,
                AdministratorId = 1   // bitno: > 0 i neka postoji admin s tim Id-jem
            };

            // Act
            var response = await Client.PostAsJsonAsync(BaseUrl, dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var created = await response.Content.ReadFromJsonAsync<HistoricalMonumentDTO>();
            created.Should().NotBeNull();
            created!.Id.Should().BeGreaterThan(0);
            created.Name.Should().Be(dto.Name);
            created.Description.Should().Be(dto.Description);
        }

        [Fact]
        public async Task Update_and_delete_historical_monument_succeed()
        {
            // 1) prvo kreiramo spomenik koji ćemo menjati i brisati
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

            var createResponse = await Client.PostAsJsonAsync(BaseUrl, createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var created =
                await createResponse.Content.ReadFromJsonAsync<HistoricalMonumentDTO>();

            created.Should().NotBeNull();
            var id = created!.Id;

            // 2) UPDATE
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

            var updateResponse = await Client.PutAsJsonAsync($"{BaseUrl}/{id}", updateDto);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updated =
                await updateResponse.Content.ReadFromJsonAsync<HistoricalMonumentDTO>();

            updated.Should().NotBeNull();
            updated!.Id.Should().Be(id);
            updated.Name.Should().Be("After update");
            updated.Status.Should().Be(MonumentStatusDTO.Inactive);

            // 3) DELETE
            var deleteResponse = await Client.DeleteAsync($"{BaseUrl}/{id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}

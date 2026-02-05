using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class HistoricalMonumentControllerTests : BaseToursIntegrationTest
    {
        public HistoricalMonumentControllerTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void GetAll_returns_seeded_monuments()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var action = controller.GetAll(1, 10);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var page = result!.Value as PagedResult<HistoricalMonumentDTO>;
            page.ShouldNotBeNull();
            page.Results.ShouldNotBeEmpty();
            page.Results.Any(m => m.Id == -1).ShouldBeTrue();
        }

        [Fact]
        public void Create_update_and_delete_monument()
        {
            long createdId;

            using (var createScope = Factory.Services.CreateScope())
            {
                var controller = CreateController(createScope);

                var createDto = new HistoricalMonumentDTO
                {
                    Name = "Controller monument",
                    Description = "Controller description",
                    YearOfCreation = 1900,
                    Status = MonumentStatusDTO.Active,
                    Latitude = 45.25,
                    Longitude = 19.85,
                    AdministratorId = 1
                };

                var createAction = controller.Create(createDto);
                var createOk = createAction.Result as OkObjectResult;
                createOk.ShouldNotBeNull();
                var created = createOk!.Value as HistoricalMonumentDTO;
                created.ShouldNotBeNull();
                createdId = created!.Id;
            }

            using (var updateScope = Factory.Services.CreateScope())
            {
                var controller = CreateController(updateScope);
                var updateDto = new HistoricalMonumentDTO
                {
                    Id = createdId,
                    Name = "Controller monument updated",
                    Description = "Controller description",
                    YearOfCreation = 1900,
                    Status = MonumentStatusDTO.Active,
                    Latitude = 45.25,
                    Longitude = 19.85,
                    AdministratorId = 1
                };

                var updateAction = controller.Update(createdId, updateDto);
                var updateOk = updateAction.Result as OkObjectResult;
                updateOk.ShouldNotBeNull();
                var updated = updateOk!.Value as HistoricalMonumentDTO;
                updated.ShouldNotBeNull();
                updated!.Name.ShouldBe("Controller monument updated");
            }

            using (var deleteScope = Factory.Services.CreateScope())
            {
                var controller = CreateController(deleteScope);
                var deleteResult = controller.Delete(createdId) as OkResult;
                deleteResult.ShouldNotBeNull();
            }
        }

        private static HistoricalMonumentController CreateController(IServiceScope scope)
        {
            return new HistoricalMonumentController(
                scope.ServiceProvider.GetRequiredService<IHistoricalMonumentService>());
        }
    }
}

using System.Linq;
using Explorer.API.Controllers;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class PublicTourControllerTests : BaseToursIntegrationTest
    {
        public PublicTourControllerTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void GetById_returns_tour_with_average_grade()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var action = controller.GetById(-2);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var dto = result!.Value as TourDto;
            dto.ShouldNotBeNull();
            dto.Id.ShouldBe(-2);
            dto.AverageGrade.ShouldBe("4.0");
        }

        [Fact]
        public void GetAll_returns_published_tours_with_average_grade()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var action = controller.GetAll(1, 10, null, null, null, null, null, null, null);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var page = result!.Value as PagedResult<TourDto>;
            page.ShouldNotBeNull();
            page.TotalCount.ShouldBe(2);
            page.Results.Count.ShouldBe(2);

            var byId = page.Results.ToDictionary(t => t.Id, t => t);
            byId[-2].AverageGrade.ShouldBe("4.0");
            byId[-4].AverageGrade.ShouldBe("No reviews");
        }

        [Fact]
        public void GetTags_returns_distinct_tags()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var action = controller.GetTags();
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var tags = result!.Value as IEnumerable<string>;
            tags.ShouldNotBeNull();
            tags!.ShouldContain("planina");
        }

        private static PublicTourController CreateController(IServiceScope scope)
        {
            return new PublicTourController(
                scope.ServiceProvider.GetRequiredService<ITourService>(),
                scope.ServiceProvider.GetRequiredService<ITourReviewService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}

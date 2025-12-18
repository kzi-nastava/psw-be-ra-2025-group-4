using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourReviewQueryTests : BaseToursIntegrationTest
    {
        public TourReviewQueryTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_all_by_tourist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = ((ObjectResult)controller.GetAll(1, 10).Result)?.Value as PagedResult<TourReviewDTO>;

            result.ShouldNotBeNull();
            result.Results.Count.ShouldBe(2);
            result.TotalCount.ShouldBe(2);
        }

        [Fact]
        public void Retrieves_all_by_tour()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = ((ObjectResult)controller.GetByTour(-1, 1, 10).Result)?.Value as PagedResult<TourReviewDTO>;

            result.ShouldNotBeNull();
            result.Results.Count.ShouldBe(2);
            result.TotalCount.ShouldBe(2);
        }

        [Fact]
        public void Retrieves_by_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as TourReviewDTO;

            result.ShouldNotBeNull();
            result.Id.ShouldBe(-1);
            result.Comment.ShouldBe("Odlična tura, preporučujem svima!");
            result.Rating.ShouldBe(5);
            result.TourCompletionPercentage.ShouldBe(100.0);
        }

        private static TourReviewController CreateController(IServiceScope scope)
        {
            return new TourReviewController(
                scope.ServiceProvider.GetRequiredService<ITourReviewService>(),
                scope.ServiceProvider.GetRequiredService<IUserService>()
            )
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}
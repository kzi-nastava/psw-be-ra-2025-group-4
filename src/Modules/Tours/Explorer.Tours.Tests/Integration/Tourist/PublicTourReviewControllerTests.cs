using Explorer.API.Controllers;
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
    public class PublicTourReviewControllerTests : BaseToursIntegrationTest
    {
        public PublicTourReviewControllerTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void GetByTour_returns_paged_reviews()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var action = controller.GetByTour(-1, 1, 10);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var page = result!.Value as PagedResult<TourReviewDTO>;
            page.ShouldNotBeNull();
            page.Results.Count.ShouldBe(2);
            page.TotalCount.ShouldBe(2);
        }

        [Fact]
        public void GetById_returns_review()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var action = controller.GetById(-1);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var dto = result!.Value as TourReviewDTO;
            dto.ShouldNotBeNull();
            dto.Id.ShouldBe(-1);
            dto.Comment.ShouldBe("Odlična tura, preporučujem svima!");
        }

        private static PublicTourReviewController CreateController(IServiceScope scope)
        {
            return new PublicTourReviewController(
                scope.ServiceProvider.GetRequiredService<ITourReviewService>(),
                scope.ServiceProvider.GetRequiredService<IUserService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}

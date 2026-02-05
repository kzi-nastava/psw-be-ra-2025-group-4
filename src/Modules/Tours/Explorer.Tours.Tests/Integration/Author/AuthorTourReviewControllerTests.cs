using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author
{
    [Collection("Sequential")]
    public class AuthorTourReviewControllerTests : BaseToursIntegrationTest
    {
        public AuthorTourReviewControllerTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void GetByTour_returns_reviews_for_author_tour()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-11");

            var action = controller.GetByTour(-2, 1, 10);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();

            var page = result!.Value as PagedResult<TourReviewDTO>;
            page.ShouldNotBeNull();
            page.Results.Count.ShouldBe(2);
            page.Results.Any(r => !string.IsNullOrWhiteSpace(r.TouristUsername)).ShouldBeTrue();
        }

        [Fact]
        public void GetByTour_returns_forbid_for_other_author()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-12");

            var action = controller.GetByTour(-2, 1, 10);
            action.Result.ShouldBeOfType<ForbidResult>();
        }

        private static TourReviewController CreateController(IServiceScope scope, string authorId)
        {
            return new TourReviewController(
                scope.ServiceProvider.GetRequiredService<ITourReviewService>(),
                scope.ServiceProvider.GetRequiredService<IUserService>(),
                scope.ServiceProvider.GetRequiredService<ITourService>())
            {
                ControllerContext = BuildContext(authorId)
            };
        }
    }
}

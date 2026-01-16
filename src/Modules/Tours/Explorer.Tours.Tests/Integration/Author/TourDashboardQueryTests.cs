using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourDashboardQueryTests : BaseToursIntegrationTest
{
    public TourDashboardQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Get_dashboard_returns_all_author_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var action = controller.GetDashboard(page: 1, pageSize: 10);

        var ok = action.Result as OkObjectResult;
        ok.ShouldNotBeNull();
        ok.StatusCode.ShouldBe(200);

        var paged = ok.Value as PagedResult<AuthorTourDashboardItemDto>;
        paged.ShouldNotBeNull();

        paged.TotalCount.ShouldBe(4);

        var ids = paged.Results.Select(r => r.TourId).ToList();
        ids.ShouldContain(-1);
        ids.ShouldContain(-2);
        ids.ShouldContain(-3);
        ids.ShouldContain(-4);
    }

    [Fact]
    public void Get_dashboard_contains_valid_statistics()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var action = controller.GetDashboard(page: 1, pageSize: 10);

        var ok = action.Result as OkObjectResult;
        ok.ShouldNotBeNull();

        var paged = ok.Value as PagedResult<AuthorTourDashboardItemDto>;
        paged.ShouldNotBeNull();

        var tour = paged.Results.First(r => r.TourId == -2);

        tour.Name.ShouldBe("Test Tura 2");
        tour.Price.ShouldBeGreaterThan(0);
        tour.LengthInKm.ShouldBeGreaterThanOrEqualTo(0);

        tour.Starts.ShouldBeGreaterThanOrEqualTo(0);
        tour.Completed.ShouldBeGreaterThanOrEqualTo(0);
        tour.Abandoned.ShouldBeGreaterThanOrEqualTo(0);
        tour.Active.ShouldBeGreaterThanOrEqualTo(0);

        tour.Popularity.ShouldBeGreaterThanOrEqualTo(0);
        tour.RatingsCount.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void Get_dashboard_details_returns_details_for_own_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-11");

        var action = controller.GetDashboardDetails(id: -2, days: 30);

        var ok = action.Result as OkObjectResult;
        ok.ShouldNotBeNull();
        ok.StatusCode.ShouldBe(200);

        var details = ok.Value as AuthorTourDashboardDetailsDto;
        details.ShouldNotBeNull();

        details.Tour.Id.ShouldBe(-2);
        details.Tour.Name.ShouldBe("Test Tura 2");

        details.Popularity.ShouldBeGreaterThanOrEqualTo(0);
        details.RatingsCount.ShouldBeGreaterThanOrEqualTo(0);

        details.Starts.ShouldBeGreaterThanOrEqualTo(0);
        details.Completed.ShouldBeGreaterThanOrEqualTo(0);
        details.Abandoned.ShouldBeGreaterThanOrEqualTo(0);
        details.Active.ShouldBeGreaterThanOrEqualTo(0);

        details.LatestReviews.Count.ShouldBeLessThanOrEqualTo(5);

        details.StartsTrend.ShouldNotBeNull();
        details.CompletedTrend.ShouldNotBeNull();
        details.AbandonedTrend.ShouldNotBeNull();
    }

    [Fact]
    public void Get_dashboard_details_fails_for_foreign_author()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-12");

        // Pošto controller hvata exception i vraća status? Kod tebe servis baca ForbiddenException,
        // a controller nema try/catch -> u unit test poziv će baciti exception.
        Should.Throw<ForbiddenException>(() => controller.GetDashboardDetails(id: -2, days: 30));
    }

    private static TourController CreateController(IServiceScope scope, string authorId)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext(authorId)
        };
    }
}

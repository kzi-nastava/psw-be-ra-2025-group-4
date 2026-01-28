using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class FavoriteTourTests : BaseToursIntegrationTest
{
    public FavoriteTourTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void AddFavorite_creates_favorite_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var result = ((ObjectResult)controller.AddFavorite(-1).Result)?.Value as FavoriteTourDto;

        result.ShouldNotBeNull();
        result.TourId.ShouldBe(-1);
        result.TouristId.ShouldBe(-1);

        var stored = dbContext.FavoriteTours
            .FirstOrDefault(ft => ft.TouristId == -1 && ft.TourId == -1);
        stored.ShouldNotBeNull();
    }

    [Fact]
    public void AddFavorite_returns_existing_when_already_favorited()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var firstResult = ((ObjectResult)controller.AddFavorite(-2).Result)?.Value as FavoriteTourDto;
        firstResult.ShouldNotBeNull();

        var secondResult = ((ObjectResult)controller.AddFavorite(-2).Result)?.Value as FavoriteTourDto;

        secondResult.ShouldNotBeNull();
        secondResult.Id.ShouldBe(firstResult.Id);

        var count = dbContext.FavoriteTours
            .Count(ft => ft.TouristId == -1 && ft.TourId == -2);
        count.ShouldBe(1);
    }

    [Fact]
    public void RemoveFavorite_deletes_favorite_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        controller.AddFavorite(-3);
        var beforeCount = dbContext.FavoriteTours
            .Count(ft => ft.TouristId == -1 && ft.TourId == -3);
        beforeCount.ShouldBe(1);

        var result = controller.RemoveFavorite(-3) as NoContentResult;

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(204);

        var stored = dbContext.FavoriteTours
            .FirstOrDefault(ft => ft.TouristId == -1 && ft.TourId == -3);
        stored.ShouldBeNull();
    }

    [Fact]
    public void IsFavorite_returns_true_when_tour_is_favorited()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        controller.AddFavorite(-4);

        var result = ((ObjectResult)controller.IsFavorite(-4).Result)?.Value as bool?;

        result.ShouldNotBeNull();
        result.Value.ShouldBeTrue();
    }

    [Fact]
    public void IsFavorite_returns_false_when_tour_is_not_favorited()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        var result = ((ObjectResult)controller.IsFavorite(-999).Result)?.Value as bool?;

        result.ShouldNotBeNull();
        result.Value.ShouldBeFalse();
    }

    [Fact]
    public void GetFavoriteTours_returns_paged_favorite_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        controller.AddFavorite(-1);
        controller.AddFavorite(-2);
        controller.AddFavorite(-3);

        var result = ((ObjectResult)controller.GetFavoriteTours(1, 10).Result)?.Value as PagedResult<TourDto>;

        result.ShouldNotBeNull();
        result.Results.Count.ShouldBeGreaterThanOrEqualTo(3);
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
        
        var tourIds = result.Results.Select(t => t.Id).ToList();
        tourIds.ShouldContain(-1);
        tourIds.ShouldContain(-2);
        tourIds.ShouldContain(-3);
    }

    [Fact]
    public void GetFavoriteTours_returns_empty_when_no_favorites()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-999");

        var result = ((ObjectResult)controller.GetFavoriteTours(1, 10).Result)?.Value as PagedResult<TourDto>;

        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(0);
        result.TotalCount.ShouldBe(0);
    }

    [Fact]
    public void GetFavoriteTours_respects_pagination()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-1");

        controller.AddFavorite(-1);
        controller.AddFavorite(-2);
        controller.AddFavorite(-3);
        controller.AddFavorite(-4);

        var result = ((ObjectResult)controller.GetFavoriteTours(1, 2).Result)?.Value as PagedResult<TourDto>;

        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(2);
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(4);
    }

    private static FavoriteTourController CreateController(IServiceScope scope, string personId)
    {
        return new FavoriteTourController(
            scope.ServiceProvider.GetRequiredService<IFavoriteTourService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }
}

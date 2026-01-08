using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class PopularToursQueryTests : BaseToursIntegrationTest
{
    public PopularToursQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_popular_tours_globally_sorted_by_popularity_desc()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // global: bez lat/lon/radius
        var result = ((ObjectResult)controller.GetPopular(page: 1, pageSize: 10, lat: null, lon: null, radiusKm: null).Result)
            ?.Value as PagedResult<PopularTourDto>;

        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Results.Count.ShouldBeGreaterThan(0);

        // Seed facts:
        // Tour -1 ima reviews (5,3) => avg 4.0
        // Tour -2 ima reviews (4,4) => avg 4.0
        // Ostale imaju 0.0
        // Sort: Popularity desc, pa RatingsCount desc, pa TourId asc
        // Pošto su -2 i -1 tied (4.0, 2), TourId asc => -2 pa -1 (jer -2 < -1)
        result.Results[0].TourId.ShouldBe(-2);
        result.Results[0].Popularity.ShouldBe(4.0, 0.001);
        result.Results[0].RatingsCount.ShouldBe(2);

        result.Results[1].TourId.ShouldBe(-1);
        result.Results[1].Popularity.ShouldBe(4.0, 0.001);
        result.Results[1].RatingsCount.ShouldBe(2);

        
        result.Results.Skip(2).All(x => x.Popularity == 0.0).ShouldBeTrue();
    }

    [Fact]
    public void Gets_popular_tours_filtered_by_radius()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        
        const double lat = 44.8200;
        const double lon = 20.4530;

        
        const double radiusKm = 1.0;

        var result = ((ObjectResult)controller.GetPopular(page: 1, pageSize: 10, lat: lat, lon: lon, radiusKm: radiusKm).Result)
            ?.Value as PagedResult<PopularTourDto>;

        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(2);
        result.Results.Count.ShouldBe(2);

     
        result.Results[0].TourId.ShouldBe(-2);
        result.Results[1].TourId.ShouldBe(-1);

     
        result.Results.All(x => x.DistanceKm.HasValue).ShouldBeTrue();
        result.Results.All(x => x.DistanceKm!.Value <= radiusKm + 0.001).ShouldBeTrue();
    }

    [Fact]
    public void Popular_tours_is_paginated()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetPopular(page: 1, pageSize: 2, lat: null, lon: null, radiusKm: null).Result)
            ?.Value as PagedResult<PopularTourDto>;

        result.ShouldNotBeNull();

        
        result.TotalCount.ShouldBe(4);
        result.Results.Count.ShouldBe(2);
    }

    [Fact]
    public void Does_not_include_other_authors_draft_tours()
    {
        using var scope = Factory.Services.CreateScope();

        
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var foreignDraft = new Tour(
            id: -999,
            name: "Foreign draft",
            description: "Should not be visible",
            difficulty: TourDifficulty.Easy,
            tags: new List<string> { "x" },
            status: TourStatus.Draft,
            authorId: -12,
            points: new List<TourPoint>(), // nema veze za global
            equipment: new List<Equipment>(),
            price: 0m,
            transportDuration: new List<TourTransportDuration>(),
            publishedAt: null,
            archivedAt: null,
            lengthInKm: 0.0
        );

        db.Tours.Add(foreignDraft);
        db.SaveChanges();

        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetPopular(page: 1, pageSize: 50, lat: null, lon: null, radiusKm: null).Result)
            ?.Value as PagedResult<PopularTourDto>;

        result.ShouldNotBeNull();

       
        result.Results.Any(x => x.TourId == -999).ShouldBeFalse();
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-11") // author id iz seed-a
        };
    }
}

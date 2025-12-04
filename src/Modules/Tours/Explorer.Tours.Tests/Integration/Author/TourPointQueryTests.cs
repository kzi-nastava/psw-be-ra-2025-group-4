using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourPointQueryTests : BaseToursIntegrationTest
{
    public TourPointQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_points_for_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var createDto = new TourPointDto
        {
            TourId = -1,
            Name = "Query point",
            Description = "For query test",
            Latitude = 45.0,
            Longitude = 19.0,
            Order = 0,
            ImageFileName = "query.jpg",
            Secret = "Query secret"
        };

        var created = ((ObjectResult)controller.CreatePoint(-1, createDto).Result)?.Value as TourPointDto;
        created.ShouldNotBeNull();

        var result = ((ObjectResult)controller.GetPoints(-1).Result)?.Value as List<TourPointDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.Any(p => p.Id == created.Id).ShouldBeTrue();
    }

    private static TourPointsController CreateController(IServiceScope scope)
    {
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        return new TourPointsController(
            scope.ServiceProvider.GetRequiredService<ITourPointService>(),
            env)
        {
            ControllerContext = BuildContext("-11")
        };
    }
}

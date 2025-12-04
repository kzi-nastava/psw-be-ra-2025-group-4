using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourPointCommandTests : BaseToursIntegrationTest
{
    public TourPointCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_point_for_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new TourPointDto
        {
            TourId = -1,
            Name = "Start point",
            Description = "First point on the tour",
            Latitude = 45.0,
            Longitude = 19.0,
            Order = 0,
            ImageFileName = "test.jpg",
            Secret = "Hidden story"
        };

        var result = ((ObjectResult)controller.CreatePoint(-1, dto).Result)?.Value as TourPointDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.TourId.ShouldBe(-1);
        result.Name.ShouldBe(dto.Name);
        result.Secret.ShouldBe(dto.Secret);

        var stored = dbContext.TourPoints.FirstOrDefault(p => p.Id == result.Id);
        stored.ShouldNotBeNull();
        stored.Name.ShouldBe(dto.Name);
        stored.Secret.ShouldBe(dto.Secret);
    }

    [Fact]
    public void Updates_point()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var createDto = new TourPointDto
        {
            TourId = -1,
            Name = "Temp point",
            Description = "Temp",
            Latitude = 45.1,
            Longitude = 19.1,
            Order = 0,
            ImageFileName = "temp.jpg",
            Secret = "Temp secret"
        };

        var created = ((ObjectResult)controller.CreatePoint(-1, createDto).Result)?.Value as TourPointDto;
        created.ShouldNotBeNull();

        var updateDto = new TourPointDto
        {
            TourId = -1,
            Name = "Updated point",
            Description = "Updated description",
            Latitude = 45.2,
            Longitude = 19.2,
            Order = created.Order,
            ImageFileName = "updated.jpg",
            Secret = "Updated secret"
        };

        var result = ((ObjectResult)controller.UpdatePoint(created.Id, updateDto).Result)?.Value as TourPointDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe(updateDto.Name);
        result.Description.ShouldBe(updateDto.Description);
        result.Secret.ShouldBe(updateDto.Secret);

        var stored = dbContext.TourPoints.FirstOrDefault(p => p.Id == created.Id);
        stored.ShouldNotBeNull();
        stored.Name.ShouldBe(updateDto.Name);
        stored.Description.ShouldBe(updateDto.Description);
        stored.Secret.ShouldBe(updateDto.Secret);
    }

    [Fact]
    public void Update_point_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new TourPointDto
        {
            TourId = -1,
            Name = "Test",
            Description = "Test",
            Latitude = 45.0,
            Longitude = 19.0,
            Order = 1,
            ImageFileName = "test.jpg",
            Secret = "Secret"
        };

        Should.Throw<NotFoundException>(() => controller.UpdatePoint(-1000, dto));
    }

    [Fact]
    public void Deletes_point()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new TourPointDto
        {
            TourId = -1,
            Name = "To delete",
            Description = "To delete",
            Latitude = 45.3,
            Longitude = 19.3,
            Order = 0,
            ImageFileName = "delete.jpg",
            Secret = "To be deleted"
        };

        var created = ((ObjectResult)controller.CreatePoint(-1, dto).Result)?.Value as TourPointDto;
        created.ShouldNotBeNull();

        var result = (NoContentResult)controller.DeletePoint(created.Id);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(204);

        var stored = dbContext.TourPoints.FirstOrDefault(p => p.Id == created.Id);
        stored.ShouldBeNull();
    }

    [Fact]
    public void Delete_point_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() => controller.DeletePoint(-1000));
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

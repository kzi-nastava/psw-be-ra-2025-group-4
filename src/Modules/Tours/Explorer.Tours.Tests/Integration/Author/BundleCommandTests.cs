using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class BundleCommandTests : BaseToursIntegrationTest
{
    public BundleCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_bundle_with_published_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var createDto = new CreateBundleDto
        {
            Name = "Summer Package",
            Price = 50.00m,
            TourIds = new List<int> { -2, -4 }
        };

        var result = ((ObjectResult)controller.Create(createDto).Result)?.Value as BundleDto;

        result.ShouldNotBeNull();
        result.Status.ShouldBe(BundleDtoStatus.Draft);
        result.Tours.Count.ShouldBe(2);

        var storedBundle = dbContext.Bundles
            .Include(b => b.Tours)
            .FirstOrDefault(b => b.Id == result.Id);
        storedBundle.ShouldNotBeNull();
        storedBundle.Status.ShouldBe(BundleStatus.Draft);
    }

    [Fact]
    public void Create_fails_with_draft_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var createDto = new CreateBundleDto
        {
            Name = "Invalid Bundle",
            Price = 30.00m,
            TourIds = new List<int> { -1 }
        };

        Should.Throw<ArgumentException>(() => controller.Create(createDto));
    }

    [Fact]
    public void Publish_succeeds_with_at_least_two_published_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var createDto = new CreateBundleDto
        {
            Name = "Published Package",
            Price = 60.00m,
            TourIds = new List<int> { -2, -4 }
        };

        var created = ((ObjectResult)controller.Create(createDto).Result)?.Value as BundleDto;
        created.ShouldNotBeNull();

        var result = (NoContentResult)controller.Publish(created.Id);

        result.StatusCode.ShouldBe(204);

        var storedBundle = dbContext.Bundles.FirstOrDefault(b => b.Id == created.Id);
        storedBundle.Status.ShouldBe(BundleStatus.Published);
    }

    [Fact]
    public void Publish_fails_with_less_than_two_published_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var createDto = new CreateBundleDto
        {
            Name = "Single Tour Package",
            Price = 30.00m,
            TourIds = new List<int> { -2 }
        };

        var created = ((ObjectResult)controller.Create(createDto).Result)?.Value as BundleDto;
        created.ShouldNotBeNull();

        Should.Throw<InvalidOperationException>(() => controller.Publish(created.Id));
    }

    [Fact]
    public void Delete_fails_for_published_bundle()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var createDto = new CreateBundleDto
        {
            Name = "Published Bundle",
            Price = 60.00m,
            TourIds = new List<int> { -2, -4 }
        };

        var created = ((ObjectResult)controller.Create(createDto).Result)?.Value as BundleDto;
        created.ShouldNotBeNull();

        controller.Publish(created.Id);

        Should.Throw<InvalidOperationException>(() => controller.Delete(created.Id));
    }

    [Fact]
    public void Archive_succeeds_for_published_bundle()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var createDto = new CreateBundleDto
        {
            Name = "To Archive Bundle",
            Price = 60.00m,
            TourIds = new List<int> { -2, -4 }
        };

        var created = ((ObjectResult)controller.Create(createDto).Result)?.Value as BundleDto;
        created.ShouldNotBeNull();

        controller.Publish(created.Id);
        var result = (NoContentResult)controller.Archive(created.Id);

        result.StatusCode.ShouldBe(204);

        var storedBundle = dbContext.Bundles.FirstOrDefault(b => b.Id == created.Id);
        storedBundle.Status.ShouldBe(BundleStatus.Archived);
    }

    private static BundleController CreateController(IServiceScope scope)
    {
        return new BundleController(scope.ServiceProvider.GetRequiredService<IBundleService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }
}

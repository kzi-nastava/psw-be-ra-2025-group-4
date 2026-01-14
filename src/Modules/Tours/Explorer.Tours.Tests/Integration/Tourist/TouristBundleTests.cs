using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Hubs;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TouristBundleTests : BaseToursIntegrationTest
{
    public TouristBundleTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void GetPublished_excludes_draft_and_archived_bundles()
    {
        using var scope = Factory.Services.CreateScope();
        var authorController = CreateAuthorController(scope);
        var touristController = CreateTouristController(scope);

        var draftDto = new CreateBundleDto
        {
            Name = "Draft Bundle",
            Price = 30.00m,
            TourIds = new List<int> { -2, -4 }
        };
        var draftCreated = ((ObjectResult)authorController.Create(draftDto).Result)?.Value as BundleDto;

        var publishedDto = new CreateBundleDto
        {
            Name = "Published Bundle",
            Price = 50.00m,
            TourIds = new List<int> { -2, -4 }
        };
        var publishedCreated = ((ObjectResult)authorController.Create(publishedDto).Result)?.Value as BundleDto;
        authorController.Publish(publishedCreated.Id);
        authorController.Archive(publishedCreated.Id);

        var result = ((ObjectResult)touristController.GetPublished(1, 10).Result)?.Value as PagedResult<BundleDto>;

        result.ShouldNotBeNull();
        result.Results.ShouldNotContain(b => b.Id == draftCreated.Id);
        result.Results.ShouldNotContain(b => b.Id == publishedCreated.Id);
    }

    [Fact]
    public void GetPublished_returns_correct_pagination()
    {
        using var scope = Factory.Services.CreateScope();
        var authorController = CreateAuthorController(scope);
        var touristController = CreateTouristController(scope);

        var bundle1 = ((ObjectResult)authorController.Create(new CreateBundleDto
        {
            Name = "Bundle 1",
            Price = 50.00m,
            TourIds = new List<int> { -2, -4 }
        }).Result)?.Value as BundleDto;
        authorController.Publish(bundle1.Id);

        var bundle2 = ((ObjectResult)authorController.Create(new CreateBundleDto
        {
            Name = "Bundle 2",
            Price = 40.00m,
            TourIds = new List<int> { -2, -4 }
        }).Result)?.Value as BundleDto;
        authorController.Publish(bundle2.Id);

        var result = ((ObjectResult)touristController.GetPublished(1, 1).Result)?.Value as PagedResult<BundleDto>;

        result.ShouldNotBeNull();
        result.Results.Count.ShouldBeLessThanOrEqualTo(1);
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void GetById_returns_published_bundle()
    {
        using var scope = Factory.Services.CreateScope();
        var authorController = CreateAuthorController(scope);
        var touristController = CreateTouristController(scope);

        var createDto = new CreateBundleDto
        {
            Name = "GetById Test Bundle",
            Price = 50.00m,
            TourIds = new List<int> { -2, -4 }
        };

        var created = ((ObjectResult)authorController.Create(createDto).Result)?.Value as BundleDto;
        created.ShouldNotBeNull();
        authorController.Publish(created.Id);

        var result = ((ObjectResult)touristController.GetById(created.Id).Result)?.Value as BundleDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe("GetById Test Bundle");
        result.Status.ShouldBe(BundleDtoStatus.Published);
        result.Tours.Count.ShouldBe(2);
    }

    [Fact]
    public void GetById_throws_when_bundle_not_published()
    {
        using var scope = Factory.Services.CreateScope();
        var authorController = CreateAuthorController(scope);
        var touristController = CreateTouristController(scope);

        var createDto = new CreateBundleDto
        {
            Name = "Draft Bundle",
            Price = 30.00m,
            TourIds = new List<int> { -2, -4 }
        };

        var created = ((ObjectResult)authorController.Create(createDto).Result)?.Value as BundleDto;
        created.ShouldNotBeNull();

        var result = touristController.GetById(created.Id).Result;

        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void IsPurchased_returns_false_when_not_purchased()
    {
        using var scope = Factory.Services.CreateScope();
        var authorController = CreateAuthorController(scope);
        var touristController = CreateTouristController(scope);

        var createDto = new CreateBundleDto
        {
            Name = "Not Purchased Bundle",
            Price = 50.00m,
            TourIds = new List<int> { -2, -4 }
        };

        var created = ((ObjectResult)authorController.Create(createDto).Result)?.Value as BundleDto;
        created.ShouldNotBeNull();
        authorController.Publish(created.Id);

        var result = ((ObjectResult)touristController.IsPurchased(created.Id).Result)?.Value as bool?;

        result.ShouldBe(false);
    }

    private static Explorer.API.Controllers.Author.BundleController CreateAuthorController(IServiceScope scope)
    {
        return new Explorer.API.Controllers.Author.BundleController(scope.ServiceProvider.GetRequiredService<IBundleService>())
        {
            ControllerContext = BuildContext("-11")
        };
    }

    private static Explorer.API.Controllers.Tourist.BundleController CreateTouristController(IServiceScope scope)
    {
        return new Explorer.API.Controllers.Tourist.BundleController(
            scope.ServiceProvider.GetRequiredService<ITouristBundleService>(),
            scope.ServiceProvider.GetRequiredService<IBundlePurchaseService>(),
            scope.ServiceProvider.GetRequiredService<INotificationService>(),
            scope.ServiceProvider.GetRequiredService<IHubContext<MessageHub>>(),
            scope.ServiceProvider.GetRequiredService<IPaymentRecordRepository>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}


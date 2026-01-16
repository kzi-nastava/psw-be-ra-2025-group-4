using System.Linq;
using Explorer.API.Controllers.Tourist.Execution;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourExecutionQueryTests : BaseToursIntegrationTest
{
    public TourExecutionQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_execution_by_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        EnsurePurchaseToken(scope, -1, -2);
        CleanupActiveExecutions(db, -1, -2);

        var createDto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2671,
            StartLongitude = 19.8335
        };
        var created = ((ObjectResult)controller.StartTour(createDto).Result)?.Value as TourExecutionDto;

        var result = ((ObjectResult)controller.GetById(created!.Id).Result)?.Value as TourExecutionDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.TourId.ShouldBe(-2);
        result.TouristId.ShouldBe(-1);
        result.Status.ShouldBe(TourExecutionStatusDto.Active);
        result.StartLatitude.ShouldBe(45.2671);
        result.StartLongitude.ShouldBe(19.8335);
    }

    [Fact]
    public void Retrieves_execution_with_next_key_point()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        EnsurePurchaseToken(scope, -1, -2);
        CleanupActiveExecutions(db, -1, -2);

        var createDto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2671,
            StartLongitude = 19.8335
        };
        var created = ((ObjectResult)controller.StartTour(createDto).Result)?.Value as TourExecutionDto;

        var result = ((ObjectResult)controller.GetById(created!.Id).Result)?.Value as TourExecutionDto;

        result.ShouldNotBeNull();
        if (result.NextKeyPoint != null)
        {
            result.NextKeyPoint.TourId.ShouldBe(-2);
        }
    }

    [Fact]
    public void GetById_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();

        Should.Throw<NotFoundException>(() => service.GetById(-9999, -1));
    }

    [Fact]
    public void GetById_fails_not_own_execution()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        EnsurePurchaseToken(scope, -1, -2);
        CleanupActiveExecutions(db, -1, -2);

        var createDto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2671,
            StartLongitude = 19.8335
        };
        var created = service.StartTour(createDto, -1);

        Should.Throw<ForbiddenException>(() => service.GetById(created.Id, -2));
    }

    private static void EnsurePurchaseToken(IServiceScope scope, int touristId, int tourId)
    {
        var paymentsDb = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
        paymentsDb.Database.EnsureCreated();

        var existingToken = paymentsDb.TourPurchaseTokens
            .FirstOrDefault(t => t.TouristId == touristId && t.TourId == tourId);

        if (existingToken == null)
        {
            paymentsDb.TourPurchaseTokens.Add(new TourPurchaseToken(touristId, tourId));
            paymentsDb.SaveChanges();
        }
    }

    private static void CleanupActiveExecutions(ToursContext db, long touristId, int tourId)
    {
        var existingExecutions = db.TourExecutions
            .Where(te => te.TouristId == touristId && te.TourId == tourId && te.Status == TourExecutionStatus.Active)
            .ToList();
        
        foreach (var existing in existingExecutions)
        {
            existing.Abandon();
            db.SaveChanges();
        }
    }

    private static TourExecutionController CreateController(IServiceScope scope)
    {
        return new TourExecutionController(scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }

    private static TourExecutionController CreateControllerForTourist(IServiceScope scope, string touristId)
    {
        return new TourExecutionController(scope.ServiceProvider.GetRequiredService<ITourExecutionService>())
        {
            ControllerContext = BuildContext(touristId)
        };
    }
}


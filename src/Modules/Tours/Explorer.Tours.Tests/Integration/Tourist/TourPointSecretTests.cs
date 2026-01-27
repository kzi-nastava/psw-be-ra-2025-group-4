using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Microsoft.EntityFrameworkCore;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Encounters.API.Public.Tourist;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourPointSecretTests : BaseToursIntegrationTest
{
    public TourPointSecretTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Gets_secret_after_completing_point()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        CleanupCompletedPoints(db, -1, -2);
        EnsurePurchaseToken(scope, -1, -2);
        var execution = CreateExecution(scope, -2);

        var tourPoint = db.TourPoints
            .FirstOrDefault(tp => tp.Id == -200);
        tourPoint.ShouldNotBeNull();

        var trackDto = new TourExecutionTrackDto
        {
            Latitude = 44.8200,
            Longitude = 20.4530
        };

        var trackController = CreateTrackController(scope);
        trackController.Track(execution.Id, trackDto);

        var actionResult = controller.GetSecret(tourPoint.Id);
        var okResult = actionResult.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        
        var secretDto = okResult.Value as TourPointSecretDto;
        secretDto.ShouldNotBeNull();
        secretDto.TourPointId.ShouldBe(tourPoint.Id);
        secretDto.IsUnlocked.ShouldBeTrue();
    }

    [Fact]
    public void Fails_to_get_secret_before_completing_point()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Tourist.ITourPointSecretService>();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        CleanupCompletedPoints(db, -1, -2);
        EnsurePurchaseToken(scope, -1, -2);
        var execution = CreateExecution(scope, -2);

        var tourPoint = db.TourPoints
            .FirstOrDefault(tp => tp.TourId == -2);
        tourPoint.ShouldNotBeNull();

        Should.Throw<ForbiddenException>(() => service.GetSecret(tourPoint.Id, -1));
    }

    [Fact]
    public void Fails_to_get_secret_for_nonexistent_point()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Tourist.ITourPointSecretService>();

        Should.Throw<NotFoundException>(() => service.GetSecret(-9999, -1));
    }

    [Fact]
    public void Gets_secret_with_null_value_when_point_has_no_secret()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        CleanupCompletedPoints(db, -1, -2);
        EnsurePurchaseToken(scope, -1, -2);
        var execution = CreateExecution(scope, -2);

        var tourPoint = db.TourPoints
            .FirstOrDefault(tp => tp.TourId == -2);
        tourPoint.ShouldNotBeNull();

        tourPoint.Update(
            tourPoint.Name,
            tourPoint.Description,
            tourPoint.Latitude,
            tourPoint.Longitude,
            tourPoint.Order,
            tourPoint.ImageFileName,
            null
        );
        db.SaveChanges();

        var trackDto = new TourExecutionTrackDto
        {
            Latitude = 44.8200,
            Longitude = 20.4530
        };

        var trackController = CreateTrackController(scope);
        trackController.Track(execution.Id, trackDto);

        var actionResult = controller.GetSecret(tourPoint.Id);
        var okResult = actionResult.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        
        var secretDto = okResult.Value as TourPointSecretDto;
        secretDto.ShouldNotBeNull();
        secretDto.TourPointId.ShouldBe(tourPoint.Id);
        secretDto.IsUnlocked.ShouldBeTrue();
        secretDto.Secret.ShouldBeNull();
    }

    private TourExecutionDto CreateExecution(IServiceScope scope, int tourId)
    {
        var service = scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Tourist.ITourExecutionService>();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
        EnsurePurchaseToken(scope, -1, tourId);
        
        var existingExecutions = db.TourExecutions
            .Where(te => te.TouristId == -1 && te.TourId == tourId && te.Status == TourExecutionStatus.Active)
            .ToList();
        
        foreach (var existing in existingExecutions)
        {
            existing.Abandon();
            db.SaveChanges();
        }
        
        var dto = new TourExecutionCreateDto
        {
            TourId = tourId,
            StartLatitude = 45.2671,
            StartLongitude = 19.8335
        };

        var result = service.StartTour(dto, -1);
        result.ShouldNotBeNull();
        return result;
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

    private static void CleanupCompletedPoints(ToursContext db, int touristId, int tourId)
    {
        var executions = db.TourExecutions
            .Include(te => te.CompletedPoints)
            .Where(te => te.TouristId == touristId && te.TourId == tourId)
            .ToList();

        foreach (var execution in executions)
        {
            if (execution.CompletedPoints.Any())
            {
                execution.CompletedPoints.Clear();
            }
        }
        
        db.SaveChanges();
    }

    private static TourPointSecretController CreateController(IServiceScope scope)
    {
        return new TourPointSecretController(
            scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Tourist.ITourPointSecretService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }

    private static Explorer.API.Controllers.Tourist.Execution.TourExecutionController CreateTrackController(IServiceScope scope)
    {
        return new Explorer.API.Controllers.Tourist.Execution.TourExecutionController(
            scope.ServiceProvider.GetRequiredService<Explorer.Tours.API.Public.Tourist.ITourExecutionService>(), scope.ServiceProvider.GetRequiredService<ITouristEncounterService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}


using System;
using System.Linq;
using Explorer.API.Controllers.Tourist.Execution;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Microsoft.EntityFrameworkCore;


namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourExecutionCommandTests : BaseToursIntegrationTest
{
    public TourExecutionCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Starts_tour_execution()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2671,
            StartLongitude = 19.8335
        };

        var result = ((ObjectResult)controller.StartTour(dto).Result)?.Value as TourExecutionDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.TourId.ShouldBe(-2);
        result.TouristId.ShouldBe(-1);
        result.Status.ShouldBe(TourExecutionStatusDto.Active);
        result.StartLatitude.ShouldBe(45.2671);
        result.StartLongitude.ShouldBe(19.8335);
        result.EndTime.ShouldBeNull();

        db.TourExecutions.Any(te => te.Id == result.Id && te.TouristId == -1).ShouldBeTrue();
    }

    [Fact]
    public void Starts_archived_tour_execution()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new TourExecutionCreateDto
        {
            TourId = -3,
            StartLatitude = 45.2671,
            StartLongitude = 19.8335
        };

        var result = ((ObjectResult)controller.StartTour(dto).Result)?.Value as TourExecutionDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.TourId.ShouldBe(-3);
        result.Status.ShouldBe(TourExecutionStatusDto.Active);
    }

    [Fact]
    public void Start_fails_draft_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();

        var dto = new TourExecutionCreateDto
        {
            TourId = -1,
            StartLatitude = 45.2671,
            StartLongitude = 19.8335
        };

        Should.Throw<InvalidOperationException>(() => service.StartTour(dto, -1));
    }

    [Fact]
    public void Start_fails_invalid_tour_id()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();

        var dto = new TourExecutionCreateDto
        {
            TourId = -9999,
            StartLatitude = 45.2671,
            StartLongitude = 19.8335
        };

        Should.Throw<NotFoundException>(() => service.StartTour(dto, -1));
    }

    [Fact]
    public void Start_fails_already_active_execution()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var existingExecutions = db.TourExecutions
            .Where(te => te.TouristId == -1 && te.TourId == -2 && te.Status == TourExecutionStatus.Active)
            .ToList();
        
        foreach (var existing in existingExecutions)
        {
            existing.Abandon();
            db.SaveChanges();
        }

        var dto = new TourExecutionCreateDto
        {
            TourId = -2,
            StartLatitude = 45.2671,
            StartLongitude = 19.8335
        };

        service.StartTour(dto, -1);

        Should.Throw<InvalidOperationException>(() => service.StartTour(dto, -1));
    }

    [Fact]
    public void Completes_tour_execution()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var execution = CreateExecution(scope, -2);


        var result = ((ObjectResult)controller.Complete(execution.Id).Result)?.Value as TourExecutionDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(execution.Id);
        result.Status.ShouldBe(TourExecutionStatusDto.Completed);
        result.EndTime.ShouldNotBeNull();

        var stored = db.TourExecutions.First(te => te.Id == execution.Id);
        stored.Status.ShouldBe(TourExecutionStatus.Completed);
        stored.EndTime.ShouldNotBeNull();
    }

    [Fact]
    public void Complete_fails_already_completed()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();

        var execution = CreateExecution(scope, -2);

        service.Complete(execution.Id, -1);

        Should.Throw<InvalidOperationException>(() => service.Complete(execution.Id, -1));
    }

    [Fact]
    public void Complete_fails_not_own_execution()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();

        var execution = CreateExecution(scope, -2);

        Should.Throw<ForbiddenException>(() => service.Complete(execution.Id, -2));
    }

    [Fact]
    public void Abandons_tour_execution()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var execution = CreateExecution(scope, -2);

        var result = ((ObjectResult)controller.Abandon(execution.Id).Result)?.Value as TourExecutionDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(execution.Id);
        result.Status.ShouldBe(TourExecutionStatusDto.Abandoned);
        result.EndTime.ShouldNotBeNull();

        var stored = db.TourExecutions.First(te => te.Id == execution.Id);
        stored.Status.ShouldBe(TourExecutionStatus.Abandoned);
        stored.EndTime.ShouldNotBeNull();
    }

    [Fact]
    public void Abandon_fails_already_completed()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();

        var execution = CreateExecution(scope, -2);
        service.Complete(execution.Id, -1);

        Should.Throw<InvalidOperationException>(() => service.Abandon(execution.Id, -1));
    }

    [Fact]
    public void Abandon_fails_not_own_execution()
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();

        var execution = CreateExecution(scope, -2);

        Should.Throw<ForbiddenException>(() => service.Abandon(execution.Id, -2));
    }

    [Fact]
    public void Tracks_location_and_completes_key_point_when_near()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var execution = CreateExecution(scope, -2);


        var trackDto = new TourExecutionTrackDto
        {
            Latitude = 44.8200,
            Longitude = 20.4530
        };

        controller.Track(execution.Id, trackDto);

        var executionFromDb = db.TourExecutions
            .Include(te => te.CompletedPoints)
            .First(te => te.Id == execution.Id);


        executionFromDb.CompletedPoints.Count.ShouldBe(1);
        executionFromDb.CompletedPoints.First().CompletedAt.ShouldNotBe(default);

        executionFromDb.LastActivity.ShouldBeGreaterThan(executionFromDb.StartTime);
    }
    [Fact]
    public void Tracks_location_without_completing_point_when_not_near()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var execution = CreateExecution(scope, -2);


        var before = db.TourExecutions.First(te => te.Id == execution.Id).LastActivity;

        var trackDto = new TourExecutionTrackDto
        {
            Latitude = 0,
            Longitude = 0
        };

        controller.Track(execution.Id, trackDto);

        var executionFromDb = db.TourExecutions
            .Include(te => te.CompletedPoints)
            .First(te => te.Id == execution.Id);


        executionFromDb.LastActivity.ShouldNotBe(before);
        executionFromDb.CompletedPoints.ShouldBeEmpty();
    }


    private TourExecutionDto CreateExecution(IServiceScope scope, int tourId)
    {
        var service = scope.ServiceProvider.GetRequiredService<ITourExecutionService>();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        
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


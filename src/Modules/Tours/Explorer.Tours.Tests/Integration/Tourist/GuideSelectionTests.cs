using System;
using System.Linq;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class GuideSelectionTests : BaseToursIntegrationTest
{
    public GuideSelectionTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_available_guides_for_tour_and_execution()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var service = scope.ServiceProvider.GetRequiredService<IGuideSelectionService>();

        var execution = CreateActiveExecution(db, -2, -1); 

        var freeGuide = new Guide("Free Guide", new[] { "SR" }, 10);
        var busyGuide = new Guide("Busy Guide", new[] { "EN" }, 20);
        db.Guides.AddRange(freeGuide, busyGuide);
        db.SaveChanges();

        db.GuideTours.Add(new GuideTour(freeGuide.Id, -2));
        db.GuideTours.Add(new GuideTour(busyGuide.Id, -2));
        db.SaveChanges();

        var otherExecution = new TourExecution(touristId: -999, tourId: -3, startLatitude: 0, startLongitude: 0);
        db.TourExecutions.Add(otherExecution);
        db.SaveChanges();

        db.GuideAssignments.Add(new GuideAssignment(busyGuide.Id, otherExecution.Id));
        db.SaveChanges();

        var result = service.GetAvailableGuides(tourId: -2, executionId: execution.Id, touristId: -1).ToList();

        result.Any(g => g.Id == freeGuide.Id).ShouldBeTrue();
        result.Any(g => g.Id == busyGuide.Id).ShouldBeFalse(); 
    }

    [Fact]
    public void SelectGuide_creates_assignment_for_execution()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var service = scope.ServiceProvider.GetRequiredService<IGuideSelectionService>();

        var execution = CreateActiveExecution(db, -2, -1);

        var guide = new Guide("Mina", new[] { "SR", "EN" }, 15);
        db.Guides.Add(guide);
        db.SaveChanges();

        db.GuideTours.Add(new GuideTour(guide.Id, -2));
        db.SaveChanges();

        service.SelectGuide(executionId: execution.Id, touristId: -1, guideId: guide.Id);

        db.GuideAssignments.Any(a =>
            a.TourExecutionId == execution.Id &&
            a.GuideId == guide.Id &&
            a.Status == GuideAssignmentStatus.Active).ShouldBeTrue();
    }

    [Fact]
    public void SelectGuide_fails_if_guide_cannot_lead_that_tour()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var service = scope.ServiceProvider.GetRequiredService<IGuideSelectionService>();

        var execution = CreateActiveExecution(db, -2, -1);

        var guide = new Guide("Ana", new[] { "EN" }, 15);
        db.Guides.Add(guide);
        db.SaveChanges();

        Should.Throw<InvalidOperationException>(() =>
            service.SelectGuide(executionId: execution.Id, touristId: -1, guideId: guide.Id));
    }

    [Fact]
    public void SelectGuide_fails_if_guide_is_busy()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var service = scope.ServiceProvider.GetRequiredService<IGuideSelectionService>();

        var execution = CreateActiveExecution(db, -2, -1);

        var guide = new Guide("Marko", new[] { "SR" }, 10);
        db.Guides.Add(guide);
        db.SaveChanges();

        db.GuideTours.Add(new GuideTour(guide.Id, -2));
        db.SaveChanges();

        var otherExecution = new TourExecution(touristId: -999, tourId: -3, startLatitude: 0, startLongitude: 0);
        db.TourExecutions.Add(otherExecution);
        db.SaveChanges();

        db.GuideAssignments.Add(new GuideAssignment(guide.Id, otherExecution.Id));
        db.SaveChanges();

        Should.Throw<InvalidOperationException>(() =>
            service.SelectGuide(executionId: execution.Id, touristId: -1, guideId: guide.Id));
    }

    private TourExecution CreateActiveExecution(ToursContext db, int tourId, long touristId = -1)
    {
        var execution = new TourExecution(
            touristId: touristId,
            tourId: tourId,
            startLatitude: 0,
            startLongitude: 0
        );

        db.TourExecutions.Add(execution);
        db.SaveChanges();
        return execution;
    }

}

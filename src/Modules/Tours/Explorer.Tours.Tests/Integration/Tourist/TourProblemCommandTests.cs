using System;
using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourProblemCommandTests : BaseToursIntegrationTest
{
    public TourProblemCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var newEntity = new TourProblemDto
        {
            TourId = -1,
            TouristId = -1,
            Category = ProblemCategoryDto.Accommodation,
            Priority = ProblemPriorityDto.High,
            Description = "Problem sa smeštajem na turi",
            Time = DateTime.SpecifyKind(new DateTime(2024, 2, 1, 12, 0, 0), DateTimeKind.Utc)
        };

        // Act
        var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Description.ShouldBe(newEntity.Description);
        result.TourId.ShouldBe(newEntity.TourId);
        result.TouristId.ShouldBe(newEntity.TouristId);

        // Assert - Database
        var storedEntity = dbContext.TourProblems.FirstOrDefault(tp => tp.Description == newEntity.Description);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var invalidEntity = new TourProblemDto
        {
            TourId = -1,
            TouristId = -1,
            Category = ProblemCategoryDto.Booking,
            Priority = ProblemPriorityDto.Low,
            Description = "", // Invalid - prazan opis
            Time = DateTime.UtcNow
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
    }

    [Fact]
    public void Updates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
        var updatedEntity = new TourProblemDto
        {
            Id = -1,
            TourId = -1,
            TouristId = -1,
            Category = ProblemCategoryDto.Booking,
            Priority = ProblemPriorityDto.High,
            Description = "Ažuriran opis problema sa rezervacijom",
            Time = DateTime.SpecifyKind(new DateTime(2024, 1, 15, 10, 30, 0), DateTimeKind.Utc)
        };

        // Act
        var result = ((ObjectResult)controller.Update(-1, updatedEntity).Result)?.Value as TourProblemDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Description.ShouldBe(updatedEntity.Description);
        result.Priority.ShouldBe(ProblemPriorityDto.High);

        // Assert - Database
        var storedEntity = dbContext.TourProblems.FirstOrDefault(tp => tp.Id == -1);
        storedEntity.ShouldNotBeNull();
        storedEntity.Description.ShouldBe(updatedEntity.Description);
        storedEntity.Priority.ShouldBe(Core.Domain.ProblemPriority.High);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new TourProblemDto
        {
            Id = -1000,
            TourId = -1,
            TouristId = -1,
            Category = ProblemCategoryDto.Other,
            Priority = ProblemPriorityDto.Low,
            Description = "Test opis",
            Time = DateTime.UtcNow
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(-1000, updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var result = (NoContentResult)controller.Delete(-3);

        // Assert - Response
        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(204);

        // Assert - Database
        var storedEntity = dbContext.TourProblems.FirstOrDefault(tp => tp.Id == -3);
        storedEntity.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }

    private static TourProblemController CreateController(IServiceScope scope)
    {
        return new TourProblemController(scope.ServiceProvider.GetRequiredService<ITourProblemService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
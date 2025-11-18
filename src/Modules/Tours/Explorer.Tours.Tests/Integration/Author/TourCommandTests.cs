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
public class TourCommandTests : BaseToursIntegrationTest
{
    public TourCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newTour = new TourDto
        {
            Name = "New tour",
            Description = "Test description",
            Difficulty = TourDtoDifficulty.Easy,
            AuthorId = 2,
            Price = 10,
            Status = 0,
            Tags = new List<string>()
        };

        var result = ((ObjectResult)controller.Create(newTour).Result)?.Value as TourDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newTour.Name);

        var storedTour = dbContext.Tours.FirstOrDefault(t => t.Id == result.Id);
        storedTour.ShouldNotBeNull();
        storedTour.Name.ShouldBe(newTour.Name);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var invalidTour = new TourDto
        {
            Description = "Missing name"
        };

        Should.Throw<ArgumentException>(() => controller.Create(invalidTour));
    }

    [Fact]
    public void Updates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var updatedTour = new TourDto
        {
            Id = 1,
            Name = "Updated tour",
            Description = "New description",
            Difficulty = TourDtoDifficulty.Medium,
            AuthorId = 2,
            Price = 20,
            Status = 0,
            Tags = new List<string>()
        };

        var result = ((ObjectResult)controller.Update(updatedTour).Result)?.Value as TourDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(updatedTour.Id);
        result.Name.ShouldBe(updatedTour.Name);
        result.Description.ShouldBe(updatedTour.Description);
        result.Difficulty.ShouldBe(updatedTour.Difficulty);

        var storedTour = dbContext.Tours.AsNoTracking().FirstOrDefault(t => t.Id == updatedTour.Id);
        storedTour.ShouldNotBeNull();
        storedTour.Name.ShouldBe(updatedTour.Name);
        storedTour.Description.ShouldBe(updatedTour.Description);
        storedTour.Difficulty.ShouldBe((TourDifficulty)updatedTour.Difficulty);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var invalidTour = new TourDto
        {
            Id = -1000,
            Name = "Test",
            Description = "Some description", 
            Difficulty = TourDtoDifficulty.Easy,
            AuthorId = 2,
            Price = 0,
            Status = 0,
            Tags = new List<string>()
        };
        ;

        Should.Throw<NotFoundException>(() => controller.Update(invalidTour));
    }

    [Fact]
    public void Deletes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var result = (NoContentResult)controller.Delete(2);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(204);

        var storedTour = dbContext.Tours.FirstOrDefault(t => t.Id == 2);
        storedTour.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(scope.ServiceProvider.GetRequiredService<ITourService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}

using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
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

        var newTour = new CreateUpdateTourDto
        {
            Name = "New tour",
            Description = "Test description",
            Difficulty = TourDtoDifficulty.Easy,
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

        var invalidTour = new CreateUpdateTourDto
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

        var dto = new CreateUpdateTourDto
        {
            Name = "Updated tour",
            Description = "New description",
            Difficulty = TourDtoDifficulty.Medium,
            Tags = new List<string>()
        };

        var result = ((ObjectResult)controller.Update(1, dto).Result)?.Value as TourDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
        result.Name.ShouldBe(dto.Name);
        result.Description.ShouldBe(dto.Description);
        result.Difficulty.ShouldBe(dto.Difficulty);

        var storedTour = dbContext.Tours.AsNoTracking().FirstOrDefault(t => t.Id == 1);
        storedTour.ShouldNotBeNull();
        storedTour.Name.ShouldBe(dto.Name);
        storedTour.Description.ShouldBe(dto.Description);
        storedTour.Difficulty.ShouldBe((TourDifficulty)dto.Difficulty);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateUpdateTourDto
        {
            Name = "Test",
            Description = "Some description",
            Difficulty = TourDtoDifficulty.Easy,
            Tags = new List<string>()
        };

        Should.Throw<NotFoundException>(() => controller.Update(-1000, dto));
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
            ControllerContext = BuildContext("2")
        };
    }
}

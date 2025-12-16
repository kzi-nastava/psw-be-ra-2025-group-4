using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourAverageGradeTests : BaseToursIntegrationTest
{
    public TourAverageGradeTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_average_grade_for_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as TourDto;

        // Assert
        result.ShouldNotBeNull();
        result.AverageGrade.ShouldBe("4.0");
    }

    [Fact]
    public void Retrieves_no_reviews_for_tour()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetById(-3).Result)?.Value as TourDto;

        // Assert
        result.ShouldNotBeNull();
        result.AverageGrade.ShouldBe("No reviews");
    }

    private static TourController CreateController(IServiceScope scope)
    {
        return new TourController(
            scope.ServiceProvider.GetRequiredService<ITourService>(),
            scope.ServiceProvider.GetRequiredService<ITourReviewService>()
        )
        {
            ControllerContext = BuildContext("-1")
        };
    }
}

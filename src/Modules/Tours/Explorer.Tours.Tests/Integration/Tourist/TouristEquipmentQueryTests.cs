using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TouristEquipmentQueryTests : BaseToursIntegrationTest
{
    public TouristEquipmentQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all_for_logged_in_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        ActionResult<List<TouristEquipmentDTO>> response = controller.GetMyEquipment();
        var okResult = response.Result as OkObjectResult;
        var result = okResult?.Value as List<TouristEquipmentDTO>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);                 
        result.Any(te => te.EquipmentId == -1).ShouldBeTrue();
        result.Any(te => te.EquipmentId == -2).ShouldBeTrue();
    }

    private static TouristEquipmentController CreateController(IServiceScope scope)
    {
        return new TouristEquipmentController(
            scope.ServiceProvider.GetRequiredService<ITouristEquipmentService>())
        {
            ControllerContext = BuildContext("-21")
        };
    }
}
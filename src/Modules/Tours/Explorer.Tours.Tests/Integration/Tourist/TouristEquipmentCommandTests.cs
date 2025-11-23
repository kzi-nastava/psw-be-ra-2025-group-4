using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TouristEquipmentCommandTests : BaseToursIntegrationTest
{
    public TouristEquipmentCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Adds_equipment_for_logged_in_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-22"); 
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        ActionResult<TouristEquipmentDTO> response = controller.AddEquipment(-1);
        var okResult = response.Result as OkObjectResult;
        var result = okResult?.Value as TouristEquipmentDTO;

        // Assert - response
        result.ShouldNotBeNull();
        result!.Id.ShouldNotBe(0);
        result.TouristId.ShouldBe(-22);
        result.EquipmentId.ShouldBe(-1);

        // Assert - DB
        var stored = db.TouristEquipment.FirstOrDefault(te =>
            te.TouristId == -22 && te.EquipmentId == -1);

        stored.ShouldNotBeNull();
    }

    [Fact]
    public void Add_fails_when_already_exists()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21"); 

        // Act & Assert
        Should.Throw<EntityValidationException>(() => controller.AddEquipment(-1));
    }

    [Fact]
    public void Removes_equipment()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        // Act
        var response = controller.RemoveEquipment(-203);
        var okResult = response as OkResult;

        // Assert - response
        okResult.ShouldNotBeNull();
        okResult!.StatusCode.ShouldBe(200);

        // Assert - DB
        var deleted = db.TouristEquipment.FirstOrDefault(te => te.Id == -203);
        deleted.ShouldBeNull();
    }

    [Fact]
    public void Remove_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "-21");

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.RemoveEquipment(-9999));
    }

    private static TouristEquipmentController CreateController(IServiceScope scope, string personId)
    {
        return new TouristEquipmentController(
            scope.ServiceProvider.GetRequiredService<ITouristEquipmentService>(),
            scope.ServiceProvider.GetRequiredService<IEquipmentService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }
}
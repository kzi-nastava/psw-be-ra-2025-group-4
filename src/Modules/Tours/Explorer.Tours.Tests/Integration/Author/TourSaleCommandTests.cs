using System;
using System.Collections.Generic;
using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class TourSaleCommandTests : BaseToursIntegrationTest
{
    public TourSaleCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_sale_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new SaleCreateDto
        {
            TourIds = new() { -2, -4 }, // Published tours from test data
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            DiscountPercent = 20
        };

        // Act
        var result = ((ObjectResult)controller.Create(dto).Result)?.Value as SaleDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
        result.DiscountPercent.ShouldBe(20);
        result.TourIds.Count.ShouldBe(2);
        result.AuthorId.ShouldBe(-11); // Author ID from test data
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Fails_to_create_sale_longer_than_two_weeks()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new SaleCreateDto
        {
            TourIds = new() { -2 }, // Published tour from test data
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(15), // > 14 days
            DiscountPercent = 15
        };

        // Act & Assert
        Should.Throw<Exception>(() => controller.Create(dto).Result);
    }

    [Fact]
    public void Fails_to_create_sale_with_invalid_discount()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new SaleCreateDto
        {
            TourIds = new() { -2 }, // Published tour from test data
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            DiscountPercent = 0 // Invalid
        };

        // Act & Assert
        Should.Throw<Exception>(() => controller.Create(dto).Result);
    }

    [Fact]
    public void Fails_to_create_sale_with_no_tours()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new SaleCreateDto
        {
            TourIds = new(), // Empty list
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            DiscountPercent = 20
        };

        // Act & Assert
        Should.Throw<Exception>(() => controller.Create(dto).Result);
    }

    [Fact]
    public void Updates_sale_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new SaleUpdateDto
        {
            TourIds = new() { -2, -4 }, // Published tours from test data
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            DiscountPercent = 25
        };

        // Act - Update sale with ID 1 (belongs to author -11)
        var result = ((ObjectResult)controller.Update(1, dto).Result)?.Value as SaleDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(1);
        result.DiscountPercent.ShouldBe(25);
        result.TourIds.Count.ShouldBe(2);
    }

    [Fact]
    public void Fails_to_update_sale_longer_than_two_weeks()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new SaleUpdateDto
        {
            TourIds = new() { -2 }, // Published tour from test data
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(20), // > 14 days
            DiscountPercent = 15
        };

        // Act & Assert
        Should.Throw<Exception>(() => controller.Update(1, dto).Result);
    }

    [Fact]
    public void Deletes_sale_successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = controller.Delete(101);

        // Assert
        result.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public void Fails_to_delete_nonexistent_sale()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<Exception>(() => controller.Delete(9999));
    }

    private static SaleController CreateController(IServiceScope scope)
    {
        return new SaleController(
            scope.ServiceProvider.GetRequiredService<ITourSaleService>()
        )
        {
            ControllerContext = BuildContext("-11") // Author ID from test data
        };
    }
}
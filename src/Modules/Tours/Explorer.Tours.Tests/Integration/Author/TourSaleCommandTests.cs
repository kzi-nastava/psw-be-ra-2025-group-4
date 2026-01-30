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
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new SaleCreateDto
        {
            TourIds = new() { -2, -4 }, 
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            DiscountPercent = 20
        };

        var result = ((ObjectResult)controller.Create(dto).Result)?.Value as SaleDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
        result.DiscountPercent.ShouldBe(20);
        result.TourIds.Count.ShouldBe(2);
        result.AuthorId.ShouldBe(-11); 
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Fails_to_create_sale_longer_than_two_weeks()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new SaleCreateDto
        {
            TourIds = new() { -2 }, 
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(15), 
            DiscountPercent = 15
        };

        Should.Throw<Exception>(() => controller.Create(dto).Result);
    }

    [Fact]
    public void Fails_to_create_sale_with_invalid_discount()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new SaleCreateDto
        {
            TourIds = new() { -2 }, 
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            DiscountPercent = 0 
        };

        Should.Throw<Exception>(() => controller.Create(dto).Result);
    }

    [Fact]
    public void Fails_to_create_sale_with_no_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new SaleCreateDto
        {
            TourIds = new(), 
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            DiscountPercent = 20
        };

        Should.Throw<Exception>(() => controller.Create(dto).Result);
    }

    [Fact]
    public void Updates_sale_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var createDto = new SaleCreateDto
        {
            TourIds = new() { -2, -4 },
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(5),
            DiscountPercent = 10
        };

        var created = ((ObjectResult)controller.Create(createDto).Result)?.Value as SaleDto;
        created.ShouldNotBeNull();

        var dto = new SaleUpdateDto
        {
            TourIds = new() { -2, -4 }, 
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            DiscountPercent = 25
        };

        var result = ((ObjectResult)controller.Update(created.Id, dto).Result)?.Value as SaleDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.DiscountPercent.ShouldBe(25);
        result.TourIds.Count.ShouldBe(2);
    }

    [Fact]
    public void Fails_to_update_sale_longer_than_two_weeks()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dto = new SaleUpdateDto
        {
            TourIds = new() { -2 }, 
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(20), 
            DiscountPercent = 15
        };

        Should.Throw<Exception>(() => controller.Update(1, dto).Result);
    }

    [Fact]
    public void Deletes_sale_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = controller.Delete(101);

        result.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public void Fails_to_delete_nonexistent_sale()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<Exception>(() => controller.Delete(9999));
    }

    private static SaleController CreateController(IServiceScope scope)
    {
        return new SaleController(
            scope.ServiceProvider.GetRequiredService<ITourSaleService>()
        )
        {
            ControllerContext = BuildContext("-11") 
        };
    }
}
using System;
using System.Linq;
using System.Collections.Generic;
using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

using TouristTourController = Explorer.API.Controllers.Tourist.TourController;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourSaleQueryTests : BaseToursIntegrationTest
{
    public TourSaleQueryTests(ToursTestFactory factory) : base(factory) { }


    [Fact]
    public void Author_retrieves_own_sales()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorSaleController(scope);

        var actionResult = controller.GetAll();
        var result = ((ObjectResult)actionResult.Result)?.Value as List<SaleDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.ShouldAllBe(s => s.AuthorId == -11); 
    }

    [Fact]
    public void Author_gets_sale_by_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorSaleController(scope);

        var actionResult = controller.GetById(100);
        var result = ((ObjectResult)actionResult.Result)?.Value as SaleDto;


        result.ShouldNotBeNull();
        result.Id.ShouldBe(100);
        result.AuthorId.ShouldBe(-11); 
        result.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Retrieves_only_tours_on_sale()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);

        var actionResult = controller.GetAll(1, 10, null, null, null, null, null, null, true);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<TourDto>;

        result.ShouldNotBeNull();
        result.Results.Count.ShouldBeGreaterThan(0);
        result.Results.ShouldAllBe(t => t.IsOnSale == true);
        result.Results.ShouldAllBe(t => t.DiscountedPrice.HasValue && t.DiscountedPrice.Value < t.OriginalPrice);
        result.Results.ShouldAllBe(t => t.SaleDiscountPercent.HasValue);
    }

    [Fact]
    public void Calculates_discounted_prices_correctly()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);

        var actionResult = controller.GetAll(1, 10, null, null, null, null, null, null, true);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<TourDto>;

        result.ShouldNotBeNull();

        foreach (var tour in result.Results)
        {
            tour.IsOnSale.ShouldBeTrue();
            tour.SaleDiscountPercent.ShouldNotBeNull();
            tour.DiscountedPrice.ShouldNotBeNull();

            var expectedDiscount = tour.OriginalPrice * tour.SaleDiscountPercent.Value / 100m;
            var expectedPrice = Math.Round(tour.OriginalPrice - expectedDiscount, 2);
            tour.DiscountedPrice.Value.ShouldBe(expectedPrice);
        }
    }

    [Fact]
    public void Sorts_by_discount_descending()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);

        var actionResult = controller.GetAll(1, 10, null, null, null, null, null, "discountdesc", true);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<TourDto>;

        result.ShouldNotBeNull();
        if (result.Results.Count > 1)
        {
            for (int i = 0; i < result.Results.Count - 1; i++)
            {
                var current = result.Results[i].SaleDiscountPercent ?? 0;
                var next = result.Results[i + 1].SaleDiscountPercent ?? 0;
                current.ShouldBeGreaterThanOrEqualTo(next);
            }
        }
        else
        {
            result.Results.Count.ShouldBeGreaterThan(0);
            result.Results[0].IsOnSale.ShouldBeTrue();
        }
    }

    [Fact]
    public void Sorts_by_discount_ascending()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);

        var actionResult = controller.GetAll(1, 10, null, null, null, null, null, "discountasc", true);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<TourDto>;

        result.ShouldNotBeNull();
        if (result.Results.Count > 1)
        {
            for (int i = 0; i < result.Results.Count - 1; i++)
            {
                var current = result.Results[i].SaleDiscountPercent ?? int.MaxValue;
                var next = result.Results[i + 1].SaleDiscountPercent ?? int.MaxValue;
                current.ShouldBeLessThanOrEqualTo(next);
            }
        }
        else
        {
            result.Results.Count.ShouldBeGreaterThan(0);
            result.Results[0].IsOnSale.ShouldBeTrue();
        }
    }

    [Fact]
    public void GetById_returns_tour_with_discount_if_on_sale()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);

        var actionResult = controller.GetById(-2);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourDto;

        result.ShouldNotBeNull();
        result.IsOnSale.ShouldBeTrue();
        result.DiscountedPrice.ShouldNotBeNull();
        result.DiscountedPrice.Value.ShouldBeLessThan(result.OriginalPrice);
        result.SaleDiscountPercent.ShouldNotBeNull();
    }

    [Fact]
    public void GetById_returns_tour_without_discount_if_not_on_sale()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);

        var actionResult = controller.GetById(-3);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourDto;

        result.ShouldNotBeNull();
        result.IsOnSale.ShouldBeFalse();
        result.DiscountedPrice.ShouldBeNull();
        result.SaleDiscountPercent.ShouldBeNull();
    }

    [Fact]
    public void Combines_sale_filter_with_price_range()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);

        var actionResult = controller.GetAll(1, 10, null, null, 0m, 500m, null, null, true);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<TourDto>;

        result.ShouldNotBeNull();
        result.Results.ShouldAllBe(t => t.IsOnSale);
        result.Results.ShouldAllBe(t => t.OriginalPrice >= 0m && t.OriginalPrice <= 500m);
    }

    [Fact]
    public void Combines_sale_filter_with_difficulty()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);

        var actionResult = controller.GetAll(1, 10, null, 0, null, null, null, null, true);
        var result = ((ObjectResult)actionResult.Result)?.Value as PagedResult<TourDto>;

        result.ShouldNotBeNull();
        if (result.Results.Any())
        {
            result.Results.ShouldAllBe(t => t.IsOnSale);
            result.Results.ShouldAllBe(t => t.Difficulty == TourDtoDifficulty.Easy);
        }
    }

    [Fact]
    public void Multiple_sales_for_same_tour_uses_highest_discount()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope);

        var actionResult = controller.GetById(-2);
        var result = ((ObjectResult)actionResult.Result)?.Value as TourDto;

        result.ShouldNotBeNull();
        result.IsOnSale.ShouldBeTrue();
        result.SaleDiscountPercent.ShouldBe(30);
    }


    private static SaleController CreateAuthorSaleController(IServiceScope scope)
    {
        return new SaleController(
            scope.ServiceProvider.GetRequiredService<ITourSaleService>()
        )
        {
            ControllerContext = BuildContext("-11")
        };
    }

    private static TouristTourController CreateTouristController(IServiceScope scope)
    {
        return new TouristTourController(
            scope.ServiceProvider.GetRequiredService<ITourService>(),
            scope.ServiceProvider.GetRequiredService<ITourReviewService>()
        )
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class TourPublishedListingTests : BaseToursIntegrationTest
{
    public TourPublishedListingTests(ToursTestFactory factory) : base(factory) { }


    [Fact]
    public void Retrieves_all_published()
    {

        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);


        var result = ((ObjectResult)controller.GetAll(
            1, 10,
            null,   
            null,   
            null,   
            null,  
            null,   
            null,   
            null    
        ).Result)?.Value as PagedResult<TourDto>;


        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(2);
        result.TotalCount.ShouldBe(2);
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

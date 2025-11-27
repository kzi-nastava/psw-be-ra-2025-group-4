using Explorer.API.Controllers.Tourist;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.BuildingBlocks.Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Blog.Tests.Integration.Tourist;

[Collection("Sequential")]
public class DigitalDiaryQueryTests : BaseBlogIntegrationTest
{
    public DigitalDiaryQueryTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all_for_tourist()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetMine(0, 0).Result)?.Value as PagedResult<DigitalDiaryDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Results.Count.ShouldBe(3);
        result.TotalCount.ShouldBe(3);
    }

    [Fact]
    public void Retrieves_by_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as DigitalDiaryDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Title.ShouldBe("Test Diary 1");
    }

    private static DigitalDiariesController CreateController(IServiceScope scope)
    {
        return new DigitalDiariesController(scope.ServiceProvider.GetRequiredService<IDigitalDiaryService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}

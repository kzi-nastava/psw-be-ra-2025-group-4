using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Blog.Tests.Integration;

[Collection("Sequential")]
public class BlogQueryTests : BaseBlogIntegrationTest
{
    public BlogQueryTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void GetMine_returns_users_blogs()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "1");
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        // Act
        var result = ((ObjectResult)controller.GetMine().Result)?.Value as IEnumerable<BlogDto>;

        // Assert
        result.ShouldNotBeNull();

        var list = result.ToList();
        list.Count.ShouldBe(4);

        list.ShouldAllBe(b => b.UserId == 1);
    }

    [Fact]
    public void Get_returns_by_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        // Act
        var result = ((ObjectResult)controller.Get(-1).Result)?.Value as BlogDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Title.ShouldBe("Test Blog 1");
        result.Description.ShouldBe("Opis bloga 1");
    }
    [Fact]
    public void Gets_active_blogs()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = new BlogController(scope.ServiceProvider.GetRequiredService<IBlogService>())
        {
            ControllerContext = BuildContext("1")
        };

        var result = ((ObjectResult)controller.GetActive().Result)?.Value as IEnumerable<BlogDto>;
        result.ShouldNotBeNull();

        var list = result.ToList();
        list.Count.ShouldBe(1);
        list[0].Id.ShouldBe(-2);
    }

    [Fact]
    public void Gets_famous_blogs()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = new BlogController(scope.ServiceProvider.GetRequiredService<IBlogService>())
        {
            ControllerContext = BuildContext("1")
        };

        var result = ((ObjectResult)controller.GetFamous().Result)?.Value as IEnumerable<BlogDto>;
        result.ShouldNotBeNull();

        var list = result.ToList();
        list.Count.ShouldBe(1);
        list[0].Id.ShouldBe(-3);
    }

    private static BlogController CreateController(IServiceScope scope, string userId = "1")
    {
        return new BlogController(scope.ServiceProvider.GetRequiredService<IBlogService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}

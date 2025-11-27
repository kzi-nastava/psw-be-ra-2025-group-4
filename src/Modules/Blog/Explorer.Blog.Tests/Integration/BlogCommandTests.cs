using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.BuildingBlocks.Core.Exceptions;


namespace Explorer.Blog.Tests.Integration;

[Collection("Sequential")]
public class BlogCommandTests : BaseBlogIntegrationTest
{
    public BlogCommandTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var dto = new CreateUpdateBlogDto
        {
            Title = "Novi Test Blog",
            Description = "Opis bloga",
            Images = new List<string> { "slika.jpg" }
        };

        var result = ((ObjectResult)controller.Create(dto).Result)?.Value as BlogDto;

        // Response assertions
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Title.ShouldBe(dto.Title);

        // DB assertions
        var stored = db.BlogPosts.FirstOrDefault(x => x.Title == dto.Title);
        stored.ShouldNotBeNull();
    }

    [Fact]
    public void Updates_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var dto = new CreateUpdateBlogDto
        {
            Title = "Izmenjen Naslov",
            Description = "Izmenjen opis",
            Images = new List<string>()
        };

        var result = ((ObjectResult)controller.Update(-1, dto).Result)?.Value as BlogDto;

        // Response assertions
        result.ShouldNotBeNull();
        result.Title.ShouldBe(dto.Title);
        result.Description.ShouldBe(dto.Description);

        // DB assertions
        var stored = db.BlogPosts.FirstOrDefault(x => x.Id == -1);
        stored.ShouldNotBeNull();
        stored.Title.ShouldBe(dto.Title);
        stored.Description.ShouldBe(dto.Description);
    }

    [Fact]
    public void Deletes_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var response = controller.Delete(-2);

        // Response assertions
        response.ShouldBeOfType<NoContentResult>();

        // DB assertions
        db.BlogPosts.FirstOrDefault(x => x.Id == -2).ShouldBeNull();
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateUpdateBlogDto
        {
            Title = "Test",
            Description = "Test desc",
            Images = new List<string>()
        };

        Should.Throw<NotFoundException>(() =>
        {
            controller.Update(-999, dto);
        });
    }

    [Fact]
    public void Update_fails_wrong_user()
    {
        using var scope = Factory.Services.CreateScope();

        // korisnik 2 pokušava da menja blog korisnika 1 (-1)
        var controller = CreateController(scope, userId: "2");

        var dto = new CreateUpdateBlogDto
        {
            Title = "Test",
            Description = "Test desc",
            Images = new List<string>()
        };

        Should.Throw<UnauthorizedAccessException>(() =>
        {
            controller.Update(-1, dto);
        });
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() =>
        {
            controller.Delete(-999);
        });
    }

    [Fact]
    public void Delete_fails_wrong_user()
    {
        using var scope = Factory.Services.CreateScope();

        // korisnik 2 pokušava da briše blog korisnika 1 (-1)
        var controller = CreateController(scope, userId: "2");

        Should.Throw<UnauthorizedAccessException>(() =>
        {
            controller.Delete(-1);
        });
    }

    private static BlogController CreateController(IServiceScope scope, string userId = "1")
    {
        return new BlogController(scope.ServiceProvider.GetRequiredService<IBlogService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }

}

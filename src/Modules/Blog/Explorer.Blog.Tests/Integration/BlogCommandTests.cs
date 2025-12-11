using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Blog.Tests.Integration;

[Collection("Sequential")]
public class BlogCommandTests : BaseBlogIntegrationTest
{
    public BlogCommandTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var dto = new CreateUpdateBlogDto
        {
            Title = "Novi blog",
            Description = "Opis",
            Images = new List<string>()
        };

        var result = ((ObjectResult)controller.Create(dto).Result)?.Value as BlogDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);

        var stored = db.BlogPosts.First(x => x.Id == result.Id);
        stored.ShouldNotBeNull();
        stored.Title.ShouldBe(dto.Title);
        stored.Status.ShouldBe(BlogStatus.Preparation);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateUpdateBlogDto
        {
            Description = "Missing title"
        };

        Should.Throw<DbUpdateException>(() => controller.Create(dto));
    }

    [Fact]
    public void Updates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var dto = new CreateUpdateBlogDto
        {
            Title = "Updated",
            Description = "New description",
            Images = new List<string>()
        };

        var result = ((ObjectResult)controller.Update(-4, dto).Result)?.Value as BlogDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(-4);

        var stored = db.BlogPosts.First(x => x.Id == -4);
        stored.ShouldNotBeNull();
        stored.Title.ShouldBe(dto.Title);
        stored.Description.ShouldBe(dto.Description);
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateUpdateBlogDto
        {
            Title = "X",
            Description = "Y",
            Images = new List<string>()
        };

        Should.Throw<NotFoundException>(() => controller.Update(-999, dto));
    }

    [Fact]
    public void Deletes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        controller.Delete(-1);

        db.BlogPosts.FirstOrDefault(x => x.Id == -1).ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_wrong_user()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, "2");

        Should.Throw<UnauthorizedAccessException>(() => controller.Delete(-1));
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() => controller.Delete(-999));
    }

    [Fact]
    public void Publishes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        controller.Publish(-1);

        var stored = db.BlogPosts.First(x => x.Id == -1);
        stored.Status.ShouldBe(BlogStatus.Published);
    }

    [Fact]
    public void Publish_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() => controller.Publish(-999));
    }

    [Fact]
    public void Archives()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        controller.Archive(-2);

        var stored = db.BlogPosts.First(x => x.Id == -2);
        stored.Status.ShouldBe(BlogStatus.Archived);
    }

    [Fact]
    public void Archive_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() => controller.Archive(-999));
    }

    private static BlogController CreateController(IServiceScope scope, string userId = "1")
    {
        return new BlogController(scope.ServiceProvider.GetRequiredService<IBlogService>())
        {
            ControllerContext = BuildContext(userId)
        };
    }
}

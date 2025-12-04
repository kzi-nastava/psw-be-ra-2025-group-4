using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Blog.Tests.Integration;

[Collection("Sequential")]
public class BlogCommandTests : BaseBlogIntegrationTest
{
    public BlogCommandTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_blog_in_preparation()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var dto = new CreateUpdateBlogDto { Title = "Novi", Description = "Opis", Images = new() };
        var result = ((ObjectResult)controller.Create(dto).Result)?.Value as BlogDto;

        result.ShouldNotBeNull();
        result.Status.ShouldBe("Preparation");
    }

    [Fact]
    public void Updates_preparation_blog_fully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var dto = new CreateUpdateBlogDto { Title = "Novo", Description = "Novo", Images = new() };
        controller.Update(-4, dto);

        var stored = db.BlogPosts.First(x => x.Id == -4);
        stored.Title.ShouldBe("Novo");
        stored.Description.ShouldBe("Novo");
    }

    [Fact]
    public void Updates_published_blog_description_only()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var original = db.BlogPosts.First(x => x.Id == -2);

        var dto = new CreateUpdateBlogDto { Title = "Zabranjeno", Description = "Dozvoljeno", Images = new() };
        controller.Update(-2, dto);

        var stored = db.BlogPosts.First(x => x.Id == -2);
        stored.Title.ShouldBe(original.Title);
        stored.Images.ShouldBe(original.Images);
        stored.Description.ShouldBe("Dozvoljeno");
        stored.LastUpdatedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Fails_update_archived_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateUpdateBlogDto { Title = "X", Description = "Y", Images = new() };

        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Update(-3, dto);
        });
    }

    [Fact]
    public void Publishes_preparation_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        controller.Publish(-1);

        var stored = db.BlogPosts.First(x => x.Id == -1);
        stored.Status.ShouldBe(BlogStatus.Published);
    }

    [Fact]
    public void Fails_publish_non_preparation_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Publish(-2);
        });

        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Publish(-3);
        });
    }

    [Fact]
    public void Archives_published_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        controller.Archive(-2);

        var stored = db.BlogPosts.First(x => x.Id == -2);
        stored.Status.ShouldBe(BlogStatus.Archived);
    }

    [Fact]
    public void Fails_archive_non_published_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Archive(-1);
        });

        Should.Throw<InvalidOperationException>(() =>
        {
            controller.Archive(-3);
        });
    }

    [Fact]
    public void Deletes_preparation_blog()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

        controller.Delete(-1);

        db.BlogPosts.FirstOrDefault(x => x.Id == -1).ShouldBeNull();
    }

    [Fact]
    public void Fails_update_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new CreateUpdateBlogDto { Title = "", Description = "", Images = new() };

        Should.Throw<NotFoundException>(() =>
        {
            controller.Update(-999, dto);
        });
    }

    [Fact]
    public void Fails_delete_wrong_user()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope, userId: "2");

        Should.Throw<UnauthorizedAccessException>(() =>
        {
            controller.Delete(-1);
        });
    }

    private static BlogController CreateController(IServiceScope scope, string userId = "1")
    {
        return new BlogController(
            scope.ServiceProvider.GetRequiredService<IBlogService>(),
            scope.ServiceProvider.GetRequiredService<ICommentService>()   // FIX
        )
        {
            ControllerContext = BuildContext(userId)
        };
    }
}

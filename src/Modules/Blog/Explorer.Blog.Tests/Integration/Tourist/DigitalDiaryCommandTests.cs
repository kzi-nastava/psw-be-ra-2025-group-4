using Explorer.API.Controllers.Tourist;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Blog.Tests.Integration.Tourist;

[Collection("Sequential")]
public class DigitalDiaryCommandTests : BaseBlogIntegrationTest
{
    public DigitalDiaryCommandTests(BlogTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var dto = new DigitalDiaryDto
        {
            Title = "New Diary",
            Country = "Serbia",
            City = "Belgrade"
        };

        var result = ((ObjectResult)controller.Create(dto).Result)?.Value as DigitalDiaryDto;

        // Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Title.ShouldBe("New Diary");

        // DB
        var stored = dbContext.DigitalDiaries.FirstOrDefault(x => x.Title == "New Diary");
        stored.ShouldNotBeNull();
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new DigitalDiaryDto
        {
            Country = "Serbia"
        };

        Should.Throw<ArgumentException>(() => controller.Create(dto));
    }

    [Fact]
    public void Updates()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var dto = new DigitalDiaryDto
        {
            Id = -1,
            Title = "Updated Diary",
            Country = "Germany",
            City = "Berlin"
        };

        var result = ((ObjectResult)controller.Update(-1, dto).Result)?.Value as DigitalDiaryDto;

        result.ShouldNotBeNull();
        result.Title.ShouldBe("Updated Diary");

        var stored = dbContext.DigitalDiaries.First(x => x.Id == -1);
        stored.Title.ShouldBe("Updated Diary");
        stored.Country.ShouldBe("Germany");
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new DigitalDiaryDto
        {
            Id = -1000,
            Title = "Invalid"
        };

        Should.Throw<NotFoundException>(() => controller.Update(-1000, dto));
    }

    [Fact]
    public void Deletes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

        var result = (OkResult)controller.Delete(-3);

        result.StatusCode.ShouldBe(200);

        var stored = dbContext.DigitalDiaries.FirstOrDefault(x => x.Id == -3);
        stored.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() => controller.Delete(-999));
    }

    private static DigitalDiariesController CreateController(IServiceScope scope)
    {
        return new DigitalDiariesController(scope.ServiceProvider.GetRequiredService<IDigitalDiaryService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
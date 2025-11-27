using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class QuizCommandTests : BaseToursIntegrationTest
{
    public QuizCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates_quiz()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var newQuiz = new QuizDto
        {
            Title = "Integration Test Quiz",
            AuthorId = "-1",
            Questions = new List<QuestionDto>()
        };

        var result = ((ObjectResult)controller.Create(newQuiz).Result)?.Value as QuizDto;

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Title.ShouldBe("Integration Test Quiz");

        db.Quizzes.Any(q => q.Id == result.Id).ShouldBeTrue();
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var invalid = new QuizDto
        {
            Title = null,
            AuthorId = "-1"
        };

        Should.Throw<ArgumentException>(() => controller.Create(invalid));
    }

    [Fact]
    public void Updates_quiz()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var created = CreateQuiz(scope, "Quiz Before Update");

        var updateDto = new QuizDto
        {
            Id = created.Id,
            Title = "Updated Quiz Title",
            AuthorId = "-1"
        };

        var result = ((ObjectResult)controller.Update(updateDto.Id, updateDto).Result)?.Value as QuizDto;

        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Title.ShouldBe("Updated Quiz Title");

        db.Quizzes.First(q => q.Id == created.Id).Title.ShouldBe("Updated Quiz Title");
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var dto = new QuizDto
        {
            Id = -9999,
            Title = "Invalid",
            AuthorId = "-1"
        };

        Should.Throw<NotFoundException>(() => controller.Update(dto.Id, dto));
    }

    [Fact]
    public void Deletes_quiz()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var created = CreateQuiz(scope, "Quiz To Delete");

        var result = (OkResult)controller.Delete(created.Id);

        result.ShouldNotBeNull();
        result.StatusCode.ShouldBe(200);

        db.Quizzes.FirstOrDefault(q => q.Id == created.Id).ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        Should.Throw<NotFoundException>(() => controller.Delete(-9999));
    }

    private QuizDto CreateQuiz(IServiceScope scope, string title)
    {
        var controller = CreateController(scope);

        var dto = new QuizDto
        {
            Title = title,
            AuthorId = "-1",
            Questions = new List<QuestionDto>()
        };

        var result = ((ObjectResult)controller.Create(dto).Result)?.Value as QuizDto;
        result.ShouldNotBeNull();

        return result!;
    }

    private static QuizController CreateController(IServiceScope scope)
    {
        return new QuizController(scope.ServiceProvider.GetRequiredService<IQuizService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
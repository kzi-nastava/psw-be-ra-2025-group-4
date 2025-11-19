using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Author;

[Collection("Sequential")]
public class QuizQueryTests : BaseToursIntegrationTest
{
    public QuizQueryTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Retrieves_all_quizzes()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Seed for this test only
        var created1 = CreateQuiz(scope, "Query Test Quiz 1");
        var created2 = CreateQuiz(scope, "Query Test Quiz 2");

        // Act
        var result = ((ObjectResult)controller.GetAll().Result)?.Value as List<QuizDto>;

        // Assert
        result.ShouldNotBeNull();
        result.ShouldContain(q => q.Id == created1.Id);
        result.ShouldContain(q => q.Id == created2.Id);
    }


    [Fact]
    public void Retrieves_quiz_by_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Create one quiz
        var created = CreateQuiz(scope, "Quiz For Single Lookup Test");

        // Act
        var result = ((ObjectResult)controller.GetById(created.Id).Result)?.Value as QuizDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Title.ShouldBe("Quiz For Single Lookup Test");
    }

    private QuizDto CreateQuiz(IServiceScope scope, string title)
    {
        var controller = CreateController(scope);

        var createDto = new QuizDto
        {
            Title = title,
            AuthorId = "-1",
            Questions = new List<QuestionDto>()
        };

        var result = ((ObjectResult)controller.Create(createDto).Result)?.Value as QuizDto;
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

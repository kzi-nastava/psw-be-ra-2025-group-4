using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
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

        // Act
        var result = ((ObjectResult)controller.GetAll().Result)?.Value as List<QuizDto>;

        // Assert
        result.ShouldNotBeNull();

        result.ShouldContain(q => q.Id == -1);
        result.ShouldContain(q => q.Id == -2);
        result.ShouldContain(q => q.Id == -3);
    }


    [Fact]
    public void Retrieves_quiz_by_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as QuizDto;

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-1);
        result.Title.ShouldBe("Existing Negative Quiz 1");
    }

    private static QuizController CreateController(IServiceScope scope)
    {
        return new QuizController(scope.ServiceProvider.GetRequiredService<IQuizService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}

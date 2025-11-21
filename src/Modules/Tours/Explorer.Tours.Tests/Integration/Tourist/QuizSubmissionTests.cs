using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist;

[Collection("Sequential")]
public class QuizSubmissionTests : BaseToursIntegrationTest
{
    public QuizSubmissionTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Submits_answers_successfully()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var submission = new QuizSubmissionDto
        {
            QuizId = -100,
            TouristId = 100,
            Answers = new List<QuizAnswerDto>
        {
            new QuizAnswerDto { QuestionId = -1000, SelectedOptionId = -10001 },
            new QuizAnswerDto { QuestionId = -1001, SelectedOptionId = -10011 }
        }
        };

        var result = ((ObjectResult)controller.SubmitAnswers(-100, submission).Result)?.Value as QuizResultDto;

        result.ShouldNotBeNull();
        result.QuizId.ShouldBe(-100);
        result.QuestionResults.ShouldNotBeNull();
        result.QuestionResults.Count.ShouldBe(3);

        var question1Result = result.QuestionResults.FirstOrDefault(r => r.QuestionId == -1000);
        question1Result.ShouldNotBeNull();
        question1Result.IsCorrect.ShouldBeTrue();
        question1Result.SelectedOptionId.ShouldBe(-10001);
        question1Result.Feedback.ShouldBe("Correct!");

        var question2Result = result.QuestionResults.FirstOrDefault(r => r.QuestionId == -1001);
        question2Result.ShouldNotBeNull();
        question2Result.IsCorrect.ShouldBeTrue();
        question2Result.SelectedOptionId.ShouldBe(-10011);
        question2Result.Feedback.ShouldBe("Correct!");

        var question3Result = result.QuestionResults.FirstOrDefault(r => r.QuestionId == -1002);
        question3Result.ShouldNotBeNull();
        question3Result.IsCorrect.ShouldBeFalse();
        question3Result.SelectedOptionId.ShouldBe(0);
        question3Result.Feedback.ShouldBe("No answer submitted");
    }

    [Fact]
    public void Submits_answers_with_incorrect_selection()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var submission = new QuizSubmissionDto
        {
            QuizId = -100,
            TouristId = 101,
            Answers = new List<QuizAnswerDto>
            {
                new QuizAnswerDto { QuestionId = -1000, SelectedOptionId = -10000 }
            }
        };

        var result = ((ObjectResult)controller.SubmitAnswers(-100, submission).Result)?.Value as QuizResultDto;

        result.ShouldNotBeNull();
        var questionResult = result.QuestionResults.FirstOrDefault(r => r.QuestionId == -1000);
        questionResult.ShouldNotBeNull();
        questionResult.IsCorrect.ShouldBeFalse();
        questionResult.SelectedOptionId.ShouldBe(-10000);
        questionResult.Feedback.ShouldContain("Incorrect");
    }

    [Fact]
    public void Submits_answers_with_multiple_correct_options()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var submission = new QuizSubmissionDto
        {
            QuizId = -100,
            TouristId = 102,
            Answers = new List<QuizAnswerDto>
            {
                new QuizAnswerDto { QuestionId = -1002, SelectedOptionId = -10020 }
            }
        };

        var result = ((ObjectResult)controller.SubmitAnswers(-100, submission).Result)?.Value as QuizResultDto;

        result.ShouldNotBeNull();
        var questionResult = result.QuestionResults.FirstOrDefault(r => r.QuestionId == -1002);
        questionResult.ShouldNotBeNull();
        questionResult.IsCorrect.ShouldBeTrue();
        questionResult.SelectedOptionId.ShouldBe(-10020);
        questionResult.Feedback.ShouldBe("Correct! Red is a color.");
    }

    [Fact]
    public void Submits_answers_with_multiple_correct_options_incorrect_selection()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var submission = new QuizSubmissionDto
        {
            QuizId = -100,
            TouristId = 103,
            Answers = new List<QuizAnswerDto>
            {
                new QuizAnswerDto { QuestionId = -1002, SelectedOptionId = -10022 }
            }
        };

        var result = ((ObjectResult)controller.SubmitAnswers(-100, submission).Result)?.Value as QuizResultDto;

        result.ShouldNotBeNull();
        var questionResult = result.QuestionResults.FirstOrDefault(r => r.QuestionId == -1002);
        questionResult.ShouldNotBeNull();
        questionResult.IsCorrect.ShouldBeFalse();
        questionResult.SelectedOptionId.ShouldBe(-10022);
        questionResult.Feedback.ShouldContain("Incorrect");
        questionResult.Feedback.ShouldContain("Correct answers");
    }

    [Fact]
    public void Submits_all_questions_correctly()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var submission = new QuizSubmissionDto
        {
            QuizId = -100,
            TouristId = 104,
            Answers = new List<QuizAnswerDto>
            {
                new QuizAnswerDto { QuestionId = -1000, SelectedOptionId = -10001 },
                new QuizAnswerDto { QuestionId = -1001, SelectedOptionId = -10011 },
                new QuizAnswerDto { QuestionId = -1002, SelectedOptionId = -10020 }
            }
        };

        var result = ((ObjectResult)controller.SubmitAnswers(-100, submission).Result)?.Value as QuizResultDto;

        result.ShouldNotBeNull();
        result.QuestionResults.Count.ShouldBe(3);
        result.QuestionResults.All(r => r.IsCorrect).ShouldBeTrue();
    }

    [Fact]
    public void Submit_fails_invalid_quiz_id()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var submission = new QuizSubmissionDto
        {
            QuizId = -9999,
            TouristId = 105,
            Answers = new List<QuizAnswerDto>()
        };

        Should.Throw<NotFoundException>(() => controller.SubmitAnswers(-9999, submission));
    }

    [Fact]
    public void Submit_fails_quiz_id_mismatch()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var submission = new QuizSubmissionDto
        {
            QuizId = -1,
            TouristId = 106,
            Answers = new List<QuizAnswerDto>()
        };

        Should.Throw<ArgumentException>(() => controller.SubmitAnswers(-100, submission));
    }

    [Fact]
    public void Submit_fails_question_not_belonging_to_quiz()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var submission = new QuizSubmissionDto
        {
            QuizId = -100,
            TouristId = 107,
            Answers = new List<QuizAnswerDto>
            {
                new QuizAnswerDto { QuestionId = -10, SelectedOptionId = -100 }
            }
        };

        Should.Throw<ArgumentException>(() => controller.SubmitAnswers(-100, submission));
    }

    private static QuizController CreateController(IServiceScope scope)
    {
        return new QuizController(
            scope.ServiceProvider.GetRequiredService<IQuizSubmissionService>(),
            scope.ServiceProvider.GetRequiredService<IQuizService>())
        {
            ControllerContext = BuildContext("1")
        };
    }
}
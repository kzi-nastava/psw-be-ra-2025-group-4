using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class OptionTests
    {
        [Fact]
        public void Constructor_with_parameters_sets_properties()
        {
            
            var text = "Option A";
            var isCorrect = true;
            var feedback = "Correct answer";
            long questionId = 42;

           
            var option = new Option(text, isCorrect, feedback, questionId);

            
            option.Text.ShouldBe(text);
            option.IsCorrect.ShouldBe(isCorrect);
            option.Feedback.ShouldBe(feedback);
            option.QuestionId.ShouldBe(questionId);
        }
    }
}

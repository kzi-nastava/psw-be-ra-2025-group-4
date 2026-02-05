using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class QuestionTests
    {
        [Fact]
        public void Constructor_with_parameters_sets_properties_and_initializes_options()
        {
            // Arrange
            var text = "Koji je glavni grad Francuske?";
            long quizId = 123;

            // Act
            var q = new Question(text, quizId);

            // Assert
            q.Text.ShouldBe(text);
            q.QuizId.ShouldBe(quizId);
            q.Options.ShouldNotBeNull();
            q.Options.Count.ShouldBe(0);
        }

        [Fact]
        public void Parameterless_constructor_initializes_options()
        {
            // Act
            var q = new Question();

            // Assert
            q.Options.ShouldNotBeNull();
        }
    }
}

using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class QuizAnswerTests
    {
        [Fact]
        public void Parameterless_constructor_creates_instance()
        {
            // Act
            var qa = new QuizAnswer();

            // Assert
            qa.ShouldNotBeNull();
        }
    }
}
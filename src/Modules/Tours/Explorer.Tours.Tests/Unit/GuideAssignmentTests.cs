using System;
using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class GuideAssignmentTests
    {
        [Fact]
        public void Cancel_when_active_should_set_status_to_cancelled()
        {
            // Arrange
            var ga = new GuideAssignment(guideId: 1, tourExecutionId: 10);

            // Act
            ga.Cancel();

            // Assert
            ga.Status.ShouldBe(GuideAssignmentStatus.Cancelled);
        }

        [Fact]
        public void Cancel_when_not_active_should_throw()
        {
            // Arrange
            var ga = new GuideAssignment(guideId: 1, tourExecutionId: 10);
            ga.Cancel(); // sad je Cancelled

            // Act & Assert
            var ex = Should.Throw<InvalidOperationException>(() => ga.Cancel());
            ex.Message.ShouldBe("Only active assignments can be cancelled.");
        }
    }
}
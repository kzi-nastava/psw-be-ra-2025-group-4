using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class TourTransportDurationTests
    {
        [Fact]
        public void Same_values_should_be_equal()
        {
            // Arrange
            var d1 = new TourTransportDuration(30, TourTransportType.Foot);
            var d2 = new TourTransportDuration(30, TourTransportType.Foot);

            // Act & Assert
            d1.ShouldBe(d2);
            d1.Equals(d2).ShouldBeTrue();
        }

        [Fact]
        public void Different_values_should_not_be_equal()
        {
            // Arrange
            var d1 = new TourTransportDuration(30, TourTransportType.Foot);
            var d2 = new TourTransportDuration(45, TourTransportType.Bike);

            // Act & Assert
            d1.ShouldNotBe(d2);
            d1.Equals(d2).ShouldBeFalse();
        }
    }
}
using Explorer.Tours.Core.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Author
{
    [Collection("Sequential")]
    public class TourUnitTests : BaseToursIntegrationTest
    {
        public TourUnitTests(ToursTestFactory factory) : base(factory) { }

        [Theory]
        [InlineData(5, true, TourStatus.Published)]
        [InlineData(-5, false, TourStatus.Draft)]
        public void Publishes(long tourId, bool shouldPublish, TourStatus expectedStatus)
        {
            var tour = GetTestTour(tourId);

            if (shouldPublish)
            {
                Should.NotThrow(() => tour.Publish());

                tour.Status.ShouldBe(expectedStatus);
                tour.PublishedAt.ShouldNotBeNull();
            }
            else
            {
                Should.Throw<ArgumentException>(() => tour.Publish());

                tour.Status.ShouldBe(TourStatus.Draft);
                tour.PublishedAt.ShouldBeNull();
            }
        }

        [Theory]
        [InlineData(1500, "RSD")]
        [InlineData(0, "RSD")]
        public void Sets_price(decimal amount, string currency)
        {
            var tour = GetTestTour(10);

            Should.NotThrow(() => tour.SetPrice(amount));

            tour.Price.ShouldBe(amount);
        }

        [Theory]
        [InlineData("Backpack", "Backpack desc")]
        [InlineData("Rope", "Rope desc")]
        public void Adds_equipment(string name, string description)
        {
            var tour = GetTestTour(10);
            var equipment = new Equipment(name, description);

            Should.NotThrow(() => tour.AddEquipment(equipment));

            tour.Equipment.ShouldContain(e => e.Name == name && e.Description == description);
        }

        [Theory]
        [InlineData("Point A", 10, 20, 1)]
        [InlineData("Point B", -5, 100, 2)]
        public void Adds_tour_point(string name, double lat, double lng, int order)
        {
            var tour = GetTestTour(10);
            var point = new TourPoint(-1, name, name, lat, lng, order, "", "");

            Should.NotThrow(() => tour.AddTourPoint(point));

            tour.Points.ShouldContain(p => p.Name == name && p.Order == order);
        }

        [Theory]
        [InlineData(TourStatus.Draft)]
        [InlineData(TourStatus.Archived)]
        public void Fails_archiving_non_published_tour(TourStatus status)
        {
            var tour = GetTestTour(10);

            tour.SetStatus(status);

            Should.Throw<InvalidOperationException>(() => tour.Archive());

            tour.Status.ShouldBe(status);
            tour.ArchivedAt.ShouldBeNull();
        }



        private static Tour GetTestTour(long tourId)
        {
            var points = tourId < 0
                ? new List<TourPoint>()
                : new List<TourPoint> { new(-1, "first", "first", 10, 20, 1, "", ""), new(-2, "second", "second", 10, 20, 2, "", "") }; // valid

            var durations = tourId < 0
                ? new List<TourTransportDuration>()
                : new List<TourTransportDuration> { new(10, Core.Domain.TourTransportType.Foot) };

            var tags = tourId < 0
                ? new List<string>()
                : new List<string> { "hiking" };

            return new Tour(tourId, "Test Tour", "A tour for testing", TourDifficulty.Easy, tags, TourStatus.Draft, 1, points, new List<Equipment>(), 1000.0m, durations, null, null);
        }

    }
}

using Explorer.Tours.Core.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Unit
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
            var tour = GetTestTour(-1);
            var point = new TourPoint(-1, name, name, lat, lng, order, "", "");

            Should.NotThrow(() => tour.AddTourPoint(point));

            tour.Points.ShouldContain(p => p.Name == name && p.Order == order);
        }

        [Fact]
        public void Updates_existing_tour_point()
        {
            var tour = GetTestTour(10);
            var existing = tour.Points.First();

            Should.NotThrow(() => tour.UpdateTourPoint(existing.Id, "Updated name", "Updated description", 11.1, 22.2, existing.Order, "updated.jpg", "Updated secret"));

            var updated = tour.Points.Single(p => p.Id == existing.Id);
            updated.Name.ShouldBe("Updated name");
            updated.Description.ShouldBe("Updated description");
            updated.Latitude.ShouldBe(11.1);
            updated.Longitude.ShouldBe(22.2);
            updated.ImageFileName.ShouldBe("updated.jpg");
            updated.Secret.ShouldBe("Updated secret");
        }

        [Fact]
        public void Update_tour_point_fails_for_unknown_id()
        {
            var tour = GetTestTour(10);

            Should.Throw<ArgumentException>(() => tour.UpdateTourPoint(9999, "Name", "Desc", 10, 20, 1, null, null));
        }

        [Fact]
        public void Removes_existing_tour_point()
        {
            var tour = GetTestTour(10);
            var existingId = tour.Points.First().Id;

            Should.NotThrow(() => tour.RemoveTourPoint(existingId));

            tour.Points.Any(p => p.Id == existingId).ShouldBeFalse();
        }

        [Fact]
        public void Remove_tour_point_fails_for_unknown_id()
        {
            var tour = GetTestTour(10);

            Should.Throw<ArgumentException>(() => tour.RemoveTourPoint(9999));
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

        [Fact]
        public void Adding_second_tour_point_increases_tour_length()
        {
            var tour = GetTestTour(-1); 

            var first = new TourPoint(-1, "A", "A", 45.0, 19.0, 1, "", "");
            var second = new TourPoint(-2, "B", "B", 45.1, 19.1, 2, "", "");

            tour.AddTourPoint(first);
            tour.LengthInKm.ShouldBe(0);

            tour.AddTourPoint(second);
            tour.LengthInKm.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Updating_point_recalculates_length()
        {
            var tour = GetTestTour(-1);

            var p1 = new TourPoint(1, "A", "A", 45.0, 19.0, 1, "", "");
            var p2 = new TourPoint(1, "B", "B", 45.1, 19.1, 2, "", "");

            tour.AddTourPoint(p1);
            tour.AddTourPoint(p2);
            var original = tour.LengthInKm;

            tour.UpdateTourPoint(p2.Id, "B2", "B2", 46.0, 20.0, 2, "", "");

            tour.LengthInKm.ShouldNotBe(original);
        }

        [Fact]
        public void Removing_point_recalculates_length()
        {
            var tour = GetTestTour(-1);

            var p1 = new TourPoint(1, "A", "A", 45.0, 19.0, 1, "", "");
            var p2 = new TourPoint(1, "B", "B", 45.1, 19.1, 2, "", "");

            tour.AddTourPoint(p1);
            tour.AddTourPoint(p2);
            tour.LengthInKm.ShouldBeGreaterThan(0);

            tour.RemoveTourPoint(p2.Id);

            tour.LengthInKm.ShouldBe(0);  
        }

        private static Tour GetTestTour(long tourId)
        {
            List<TourPoint> points;

            if (tourId < 0)
            {
                points = new List<TourPoint>();
            }
            else
            {
                var p1 = new TourPoint(tourId: tourId, name: "first", description: "first", latitude: 10, longitude: 20, order: 1, imageFileName: "", secret: "")
                {
                    Id = 1 
                };

                var p2 = new TourPoint(tourId: tourId, name: "second", description: "second", latitude: 10, longitude: 20, order: 2, imageFileName: "", secret: "")
                {
                    Id = 2  
                };

                points = new List<TourPoint> { p1, p2 };
            }

            var durations = tourId < 0
                ? new List<TourTransportDuration>()
                : new List<TourTransportDuration> { new(10, TourTransportType.Foot) };

            var tags = tourId < 0
                ? new List<string>()
                : new List<string> { "hiking" };

            return new Tour(tourId, "Test Tour", "A tour for testing", TourDifficulty.Easy, tags, TourStatus.Draft, 1, points, new List<Equipment>(), 1000.0m, durations, null, null);
        }


    }
}

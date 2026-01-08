using Explorer.Tours.Core.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Tests.Unit
{
    [Collection("Sequential")]
    public class BundleUnitTests : BaseToursIntegrationTest
    {
        public BundleUnitTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_bundle_with_draft_status()
        {
            var tours = GetPublishedTours(2);
            var bundle = new Bundle("Test Bundle", 50.00m, 1, tours);

            bundle.Status.ShouldBe(BundleStatus.Draft);
            bundle.Tours.Count.ShouldBe(2);
        }

        [Fact]
        public void Create_fails_with_empty_name()
        {
            var tours = GetPublishedTours(1);
            Should.Throw<ArgumentException>(() => new Bundle("", 50.00m, 1, tours));
        }

        [Fact]
        public void Create_fails_with_negative_price()
        {
            var tours = GetPublishedTours(1);
            Should.Throw<ArgumentException>(() => new Bundle("Test Bundle", -10.00m, 1, tours));
        }

        [Fact]
        public void Create_fails_with_no_tours()
        {
            Should.Throw<ArgumentException>(() => new Bundle("Test Bundle", 50.00m, 1, new List<Tour>()));
        }

        [Fact]
        public void Publish_succeeds_with_at_least_two_published_tours()
        {
            var tours = GetPublishedTours(2);
            var bundle = new Bundle("Test Bundle", 50.00m, 1, tours);

            Should.NotThrow(() => bundle.Publish());
            bundle.Status.ShouldBe(BundleStatus.Published);
        }

        [Fact]
        public void Publish_fails_with_less_than_two_published_tours()
        {
            var tours = GetPublishedTours(1);
            var bundle = new Bundle("Test Bundle", 50.00m, 1, tours);

            Should.Throw<InvalidOperationException>(() => bundle.Publish());
            bundle.Status.ShouldBe(BundleStatus.Draft);
        }

        [Fact]
        public void Delete_fails_for_published_bundle()
        {
            var tours = GetPublishedTours(2);
            var bundle = new Bundle("Test Bundle", 50.00m, 1, tours);
            bundle.Publish();

            Should.Throw<InvalidOperationException>(() => bundle.Delete());
        }

        [Fact]
        public void Archive_succeeds_for_published_bundle()
        {
            var tours = GetPublishedTours(2);
            var bundle = new Bundle("Test Bundle", 50.00m, 1, tours);
            bundle.Publish();

            Should.NotThrow(() => bundle.Archive());
            bundle.Status.ShouldBe(BundleStatus.Archived);
        }

        private static List<Tour> GetPublishedTours(int count)
        {
            var tours = new List<Tour>();
            for (int i = 0; i < count; i++)
            {
                var tour = new Tour(
                    id: i + 1,
                    name: $"Published Tour {i + 1}",
                    description: "Description",
                    difficulty: TourDifficulty.Easy,
                    tags: new List<string> { "test" },
                    status: TourStatus.Published,
                    authorId: 1,
                    points: new List<TourPoint>(),
                    equipment: new List<Equipment>(),
                    price: 20.00m,
                    transportDuration: new List<TourTransportDuration>(),
                    publishedAt: DateTime.UtcNow,
                    archivedAt: null,
                    lengthInKm: 0
                );
                tours.Add(tour);
            }
            return tours;
        }

        private static List<Tour> GetDraftTours(int count)
        {
            var tours = new List<Tour>();
            for (int i = 0; i < count; i++)
            {
                var tour = new Tour(
                    id: i + 100,
                    name: $"Draft Tour {i + 1}",
                    description: "Description",
                    difficulty: TourDifficulty.Easy,
                    tags: new List<string> { "test" },
                    status: TourStatus.Draft,
                    authorId: 1,
                    points: new List<TourPoint>(),
                    equipment: new List<Equipment>(),
                    price: 20.00m,
                    transportDuration: new List<TourTransportDuration>(),
                    publishedAt: null,
                    archivedAt: null,
                    lengthInKm: 0
                );
                tours.Add(tour);
            }
            return tours;
        }
    }
}


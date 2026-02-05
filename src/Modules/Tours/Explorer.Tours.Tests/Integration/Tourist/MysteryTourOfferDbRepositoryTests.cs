using System;
using System.Linq;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Infrastructure.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class MysteryTourOfferDbRepositoryTests : BaseToursIntegrationTest
    {
        public MysteryTourOfferDbRepositoryTests(ToursTestFactory factory) : base(factory) { }

        // =========================
        // COMMAND TESTS (Create/Update)
        // =========================

        [Fact]
        public void Creates_offer()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var repo = new MysteryTourOfferDbRepository(db);

            // Arrange
            var offer = new MysteryTourOffer(touristId: -21, tourId: -2, discountPercent: 20);

            // Act
            var created = repo.Create(offer);

            // Assert
            created.ShouldNotBeNull();
            created.Id.ShouldNotBe(Guid.Empty);

            var fromDb = db.MysteryTourOffers.Single(o => o.Id == created.Id);
            fromDb.TouristId.ShouldBe(-21);
            fromDb.TourId.ShouldBe(-2);
            fromDb.DiscountPercent.ShouldBe(20);
            fromDb.Redeemed.ShouldBeFalse();
        }

        [Fact]
        public void Updates_offer()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var repo = new MysteryTourOfferDbRepository(db);

            // Arrange
            var offer = new MysteryTourOffer(touristId: -21, tourId: -2, discountPercent: 20);
            repo.Create(offer);

            // Act
            offer.Redeem();
            var updated = repo.Update(offer);

            // Assert
            updated.ShouldNotBeNull();
            var fromDb = db.MysteryTourOffers.Single(o => o.Id == offer.Id);
            fromDb.Redeemed.ShouldBeTrue();
        }

        // =========================
        // QUERY TESTS (GetActiveForTourist)
        // =========================

        [Fact]
        public void GetActiveForTourist_returns_null_when_no_offers()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var repo = new MysteryTourOfferDbRepository(db);

            var result = repo.GetActiveForTourist(touristId: -999);

            result.ShouldBeNull();
        }

        

        [Fact]
        public void GetActiveForTourist_returns_latest_active_offer()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var repo = new MysteryTourOfferDbRepository(db);

            // Arrange: stariji valid
            var oldOffer = new MysteryTourOffer(touristId: -21, tourId: -2, discountPercent: 10);
            typeof(MysteryTourOffer)
                .GetProperty(nameof(MysteryTourOffer.CreatedAt))!
                .SetValue(oldOffer, DateTime.UtcNow.AddMinutes(-20));
            repo.Create(oldOffer);

            // Arrange: noviji valid
            var newOffer = new MysteryTourOffer(touristId: -21, tourId: -3, discountPercent: 30);
            repo.Create(newOffer);

            // Act
            var result = repo.GetActiveForTourist(-21);

            // Assert
            result.ShouldNotBeNull();
            result!.TourId.ShouldBe(-3);
            result.DiscountPercent.ShouldBe(30);
            result.Redeemed.ShouldBeFalse();
            result.ExpiresAt.ShouldBeGreaterThan(DateTime.UtcNow);
        }
    }
}

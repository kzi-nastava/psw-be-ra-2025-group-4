using System;
using Explorer.Tours.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class MysteryTourOfferTests
    {
        [Fact]
        public void Constructor_sets_default_values()
        {
            var offer = new MysteryTourOffer(touristId: 5, tourId: 12, discountPercent: 20);

            offer.Id.ShouldNotBe(Guid.Empty);
            offer.TouristId.ShouldBe(5);
            offer.TourId.ShouldBe(12);
            offer.DiscountPercent.ShouldBe(20);

            offer.Redeemed.ShouldBeFalse();
            offer.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
            offer.ExpiresAt.ShouldBe(offer.CreatedAt.AddMinutes(10));
        }

        [Fact]
        public void Redeem_marks_offer_as_redeemed_when_valid()
        {
            var offer = new MysteryTourOffer(1, 10, 30);

            offer.Redeem();

            offer.Redeemed.ShouldBeTrue();
        }

        [Fact]
        public void Redeem_throws_when_already_redeemed()
        {
            var offer = new MysteryTourOffer(1, 10, 30);
            offer.Redeem();

            var ex = Should.Throw<InvalidOperationException>(() => offer.Redeem());
            ex.Message.ShouldBe("Offer already redeemed.");
        }

        [Fact]
        public void Redeem_throws_when_expired()
        {
            var offer = new MysteryTourOffer(1, 10, 30);

            typeof(MysteryTourOffer)
                .GetProperty(nameof(MysteryTourOffer.ExpiresAt))!
                .SetValue(offer, DateTime.UtcNow.AddMinutes(-1));

            var ex = Should.Throw<InvalidOperationException>(() => offer.Redeem());
            ex.Message.ShouldBe("Offer expired.");
        }
    }
}

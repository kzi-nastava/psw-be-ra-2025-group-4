using System;
using Explorer.Payments.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Unit
{
    [Collection("Sequential")]
    public class AffiliateCodeUnitTests
    {
        [Fact]
        public void Creates_affiliate_code_with_valid_data()
        {
            var ac = new AffiliateCode(
                code: "ABCDEFGHJK",
                authorId: 1,
                tourId: null,
                affiliateTouristId: 55,
                percent: 10m,
                expiresAt: null);

            ac.Code.ShouldBe("ABCDEFGHJK");
            ac.AuthorId.ShouldBe(1);
            ac.TourId.ShouldBeNull();
            ac.AffiliateTouristId.ShouldBe(55);
            ac.Percent.ShouldBe(10m);
            ac.ExpiresAt.ShouldBeNull();

            ac.Active.ShouldBeTrue();
            ac.UsageCount.ShouldBe(0);
            ac.DeactivatedAt.ShouldBeNull();

            ac.CreatedAt.ShouldBeInRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));
        }

        [Fact]
        public void Create_fails_when_code_is_empty()
        {
            Should.Throw<ArgumentException>(() =>
                new AffiliateCode("", 1, null, affiliateTouristId: 55, percent: 10m, expiresAt: null));
        }

        [Fact]
        public void Create_fails_when_code_is_too_long()
        {
            var tooLong = new string('A', 17);
            Should.Throw<ArgumentException>(() =>
                new AffiliateCode(tooLong, 1, null, affiliateTouristId: 55, percent: 10m, expiresAt: null));
        }

        [Fact]
        public void Create_fails_when_affiliate_tourist_is_missing()
        {
            Should.Throw<ArgumentException>(() =>
                new AffiliateCode("ABCDEFGHJK", 1, null, affiliateTouristId: 0, percent: 10m, expiresAt: null));
        }

        [Fact]
        public void Create_fails_when_percent_is_out_of_range()
        {
            Should.Throw<ArgumentException>(() =>
                new AffiliateCode("ABCDEFGHJK", 1, null, affiliateTouristId: 55, percent: 0m, expiresAt: null));

            Should.Throw<ArgumentException>(() =>
                new AffiliateCode("ABCDEFGHJK", 1, null, affiliateTouristId: 55, percent: 100.01m, expiresAt: null));
        }

        [Fact]
        public void Create_fails_when_expiration_is_in_the_past()
        {
            Should.Throw<ArgumentException>(() =>
                new AffiliateCode("ABCDEFGHJK", 1, null, affiliateTouristId: 55, percent: 10m, expiresAt: DateTime.UtcNow.AddMinutes(-1)));
        }

        [Fact]
        public void Deactivate_and_activate_work()
        {
            var ac = new AffiliateCode("ABCDEFGHJK", 1, null, affiliateTouristId: 55, percent: 10m, expiresAt: null);

            ac.Deactivate();
            ac.Active.ShouldBeFalse();
            ac.DeactivatedAt.ShouldNotBeNull();

            ac.Activate();
            ac.Active.ShouldBeTrue();
            ac.DeactivatedAt.ShouldBeNull();
        }

        [Fact]
        public void Increment_usage_works()
        {
            var ac = new AffiliateCode("ABCDEFGHJK", 1, null, affiliateTouristId: 55, percent: 10m, expiresAt: null);

            ac.UsageCount.ShouldBe(0);
            ac.IncrementUsage();
            ac.UsageCount.ShouldBe(1);
        }
    }
}

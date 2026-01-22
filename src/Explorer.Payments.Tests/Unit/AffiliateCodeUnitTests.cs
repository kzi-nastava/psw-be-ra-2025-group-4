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
            var ac = new AffiliateCode("ABCDEFGHJK", authorId: 1, tourId: null);

            ac.Code.ShouldBe("ABCDEFGHJK");
            ac.AuthorId.ShouldBe(1);
            ac.TourId.ShouldBeNull();
            ac.Active.ShouldBeTrue();
            ac.CreatedAt.ShouldBeInRange(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(1));
        }

        [Fact]
        public void Create_fails_when_code_is_empty()
        {
            Should.Throw<ArgumentException>(() => new AffiliateCode("", 1, null));
        }

        [Fact]
        public void Create_fails_when_code_is_too_long()
        {
            var tooLong = new string('A', 17); 
            Should.Throw<ArgumentException>(() => new AffiliateCode(tooLong, 1, null));
        }

        [Fact]
        public void Deactivate_and_activate_work()
        {
            var ac = new AffiliateCode("ABCDEFGHJK", 1, null);

            ac.Deactivate();
            ac.Active.ShouldBeFalse();

            ac.Activate();
            ac.Active.ShouldBeTrue();
        }
    }
}

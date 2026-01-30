using System;
using Explorer.Payments.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Unit
{
    public class TouristReferralInviteUnitTests
    {
        [Fact]
        public void Invite_created_successfully()
        {
            var invite = new TouristReferralInvite("REF123", -21);

            invite.Code.ShouldBe("REF123");
            invite.ReferrerTouristId.ShouldBe(-21);
            invite.IsUsed.ShouldBeFalse();
            invite.ReferredTouristId.ShouldBeNull();
            invite.CreatedAtUtc.ShouldBeInRange(
                DateTime.UtcNow.AddMinutes(-1),
                DateTime.UtcNow.AddMinutes(1));
        }

        [Fact]
        public void Create_fails_when_code_is_empty()
        {
            Should.Throw<ArgumentException>(() =>
                new TouristReferralInvite("", -21));
        }

        [Fact]
        public void Consume_marks_invite_as_used()
        {
            var invite = new TouristReferralInvite("REF123", -21);

            invite.Consume(-22);

            invite.IsUsed.ShouldBeTrue();
            invite.ReferredTouristId.ShouldBe(-22);
            invite.UsedAtUtc.ShouldNotBeNull();
        }

        [Fact]
        public void Consume_fails_when_used_twice()
        {
            var invite = new TouristReferralInvite("REF123", -21);
            invite.Consume(-22);

            Should.Throw<InvalidOperationException>(() =>
                invite.Consume(-23));
        }

        [Fact]
        public void Consume_fails_on_self_referral()
        {
            var invite = new TouristReferralInvite("REF123", -21);

            Should.Throw<InvalidOperationException>(() =>
                invite.Consume(-21));
        }
    }
}

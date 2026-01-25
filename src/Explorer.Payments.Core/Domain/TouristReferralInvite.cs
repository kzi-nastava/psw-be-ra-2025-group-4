using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Payments.Core.Domain
{
    public class TouristReferralInvite : AggregateRoot
    {
        public string Code { get; private set; }
        public long ReferrerTouristId { get; private set; }

        public bool IsUsed { get; private set; }
        public long? ReferredTouristId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UsedAtUtc { get; private set; }

        private TouristReferralInvite() { } // EF

        public TouristReferralInvite(string code, long referrerTouristId)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code is required.", nameof(code));
            if (referrerTouristId <= 0) throw new ArgumentException("Invalid referrer id.", nameof(referrerTouristId));

            Code = code;
            ReferrerTouristId = referrerTouristId;
            CreatedAtUtc = DateTime.UtcNow;
            IsUsed = false;
        }

        public void Consume(long referredTouristId)
        {
            if (referredTouristId <= 0) throw new ArgumentException("Invalid referred id.", nameof(referredTouristId));
            if (IsUsed) throw new InvalidOperationException("Referral code already used.");
            if (referredTouristId == ReferrerTouristId) throw new InvalidOperationException("Self-referral is not allowed.");

            IsUsed = true;
            ReferredTouristId = referredTouristId;
            UsedAtUtc = DateTime.UtcNow;
        }
    }
}

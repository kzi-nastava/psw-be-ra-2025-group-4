using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class AffiliateCode : Entity
    {
        public string Code { get; private set; } = string.Empty;
        public int AuthorId { get; private set; }
        public int? TourId { get; private set; }
        public int AffiliateTouristId { get; private set; }
        public decimal Percent { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public int UsageCount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool Active { get; private set; }
        public DateTime? DeactivatedAt { get; private set; }

        private AffiliateCode() { }

        public AffiliateCode(string code, int authorId, int? tourId, int affiliateTouristId, decimal percent, DateTime? expiresAt)
        {
            Code = code;
            AuthorId = authorId;
            TourId = tourId;

            AffiliateTouristId = affiliateTouristId;
            Percent = percent;
            ExpiresAt = expiresAt;

            CreatedAt = DateTime.UtcNow;
            Active = true;
            UsageCount = 0;

            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Code)) throw new ArgumentException("Code cannot be empty.");
            if (Code.Length > 16) throw new ArgumentException("Code too long.");

            if (AffiliateTouristId == 0) throw new ArgumentException("AffiliateTouristId is required.");
            if (Percent <= 0 || Percent > 100) throw new ArgumentException("Percent must be in (0, 100].");

            if (ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow)
                throw new ArgumentException("ExpiresAt must be in the future.");
        }

        public void Deactivate()
        {
            Active = false;
            DeactivatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            Active = true;
            DeactivatedAt = null;
        }

        public void IncrementUsage()
        {
            UsageCount++;
        }

        public bool IsExpired() => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    }
}

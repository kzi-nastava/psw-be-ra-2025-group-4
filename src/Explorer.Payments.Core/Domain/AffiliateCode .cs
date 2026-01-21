using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class AffiliateCode : Entity
    {
        public string Code { get; private set; } = string.Empty;
        public int AuthorId { get; private set; }
        public int? TourId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool Active { get; private set; }

        private AffiliateCode() { }

        public AffiliateCode(string code, int authorId, int? tourId)
        {
            Code = code;
            AuthorId = authorId;
            TourId = tourId;
            CreatedAt = DateTime.UtcNow;
            Active = true;

            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Code)) throw new ArgumentException("Code cannot be empty.");
            if (Code.Length > 16) throw new ArgumentException("Code too long.");
        }

        public void Deactivate() => Active = false;
        public void Activate() => Active = true;
    }
}

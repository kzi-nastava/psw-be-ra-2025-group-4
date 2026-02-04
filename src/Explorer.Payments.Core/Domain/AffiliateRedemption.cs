using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class AffiliateRedemption : Entity
    {
        public int AffiliateCodeId { get; private set; }
        public string Code { get; private set; } = string.Empty;

        public int AuthorId { get; private set; }
        public int TourId { get; private set; }

        public int AffiliateTouristId { get; private set; }
        public int BuyerTouristId { get; private set; }

        public decimal AmountPaid { get; private set; } 
        public decimal CommissionAmount { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private AffiliateRedemption() { }

        public AffiliateRedemption(
            int affiliateCodeId,
            string code,
            int authorId,
            int tourId,
            int affiliateTouristId,
            int buyerTouristId,
            decimal amountPaid,
            decimal commissionAmount)
        {
            if (affiliateCodeId <= 0) throw new ArgumentException("Invalid affiliateCodeId.");
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Invalid code.");
            if (authorId <= 0) throw new ArgumentException("Invalid authorId.");
            if (tourId <= 0) throw new ArgumentException("Invalid tourId.");
            if (affiliateTouristId <= 0) throw new ArgumentException("Invalid affiliateTouristId.");
            if (buyerTouristId <= 0) throw new ArgumentException("Invalid buyerTouristId.");
            if (amountPaid <= 0) throw new ArgumentException("Invalid amountPaid.");
            if (commissionAmount < 0) throw new ArgumentException("Invalid commissionAmount.");

            AffiliateCodeId = affiliateCodeId;
            Code = code.Trim();
            AuthorId = authorId;
            TourId = tourId;
            AffiliateTouristId = affiliateTouristId;
            BuyerTouristId = buyerTouristId;
            AmountPaid = amountPaid;
            CommissionAmount = commissionAmount;
            CreatedAt = DateTime.UtcNow;
        }
    }
}

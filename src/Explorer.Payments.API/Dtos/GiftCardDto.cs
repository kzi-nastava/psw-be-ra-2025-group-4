using System;

namespace Explorer.Payments.API.Dtos
{
    public class GiftCardDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = "";
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public DateTime PurchasedAt { get; set; }
        public string? SenderUsername { get; set; }
        /// <summary>Set only when returning from purchase, so the controller can send notification to the recipient.</summary>
        public int? RecipientTouristId { get; set; }
    }
}

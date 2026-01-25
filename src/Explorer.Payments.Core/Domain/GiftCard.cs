using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    /// <summary>
    /// Gift card purchased for another user. Amount and Balance are in real currency (e.g. EUR).
    /// </summary>
    public class GiftCard : Entity
    {
        public string Code { get; private set; }
        public int RecipientTouristId { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Balance { get; private set; }
        public int BuyerTouristId { get; private set; }
        public DateTime PurchasedAt { get; private set; }

        private GiftCard() { Code = null!; }

        public GiftCard(string code, int recipientTouristId, decimal amount, int buyerTouristId)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length < 10)
                throw new ArgumentException("Gift card code must be at least 10 characters.", nameof(code));
            if (recipientTouristId == 0)
                throw new ArgumentException("Invalid recipient.", nameof(recipientTouristId));
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.", nameof(amount));
            if (buyerTouristId == 0)
                throw new ArgumentException("Invalid buyer.", nameof(buyerTouristId));

            Code = code;
            RecipientTouristId = recipientTouristId;
            Amount = amount;
            Balance = amount;
            BuyerTouristId = buyerTouristId;
            PurchasedAt = DateTime.UtcNow;
        }

        public void Deduct(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deduct amount must be positive.", nameof(amount));
            if (amount > Balance)
                throw new InvalidOperationException("Insufficient gift card balance.");
            Balance -= amount;
        }

        public bool HasBalance => Balance > 0;
    }
}

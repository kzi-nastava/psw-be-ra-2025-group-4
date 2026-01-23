using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public enum PaymentMethod
    {
        CreditCard = 1,
        PayPal = 2,
        GiftCard = 3
    }

    public class CoinsBundlePurchase : Entity
    {
        public int TouristId { get; private set; }
        public int CoinsBundleId { get; private set; }
        public string BundleName { get; private set; } // Za istoriju ako se bundle promeni
        public int CoinsReceived { get; private set; }
        public decimal PricePaid { get; private set; } // Sa snižením
        public decimal OriginalPrice { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public DateTime PurchaseDate { get; private set; }
        public string TransactionId { get; private set; } // Simulirani transaction ID

        private CoinsBundlePurchase() { }

        public CoinsBundlePurchase(int touristId, int coinsBundleId, string bundleName,
            int coinsReceived, decimal pricePaid, decimal originalPrice,
            PaymentMethod paymentMethod, string transactionId)
        {
            if (touristId == 0)
                throw new ArgumentException("Invalid tourist id.", nameof(touristId));
            if (coinsBundleId <= 0)
                throw new ArgumentException("Invalid bundle id.", nameof(coinsBundleId));
            if (coinsReceived <= 0)
                throw new ArgumentException("Coins received must be positive.", nameof(coinsReceived));
            if (pricePaid <= 0)
                throw new ArgumentException("Price paid must be positive.", nameof(pricePaid));

            TouristId = touristId;
            CoinsBundleId = coinsBundleId;
            BundleName = bundleName;
            CoinsReceived = coinsReceived;
            PricePaid = pricePaid;
            OriginalPrice = originalPrice;
            PaymentMethod = paymentMethod;
            PurchaseDate = DateTime.UtcNow;
            TransactionId = transactionId;
        }
    }
}
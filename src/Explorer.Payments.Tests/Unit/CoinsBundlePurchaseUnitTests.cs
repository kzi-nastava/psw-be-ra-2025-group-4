using Explorer.Payments.Core.Domain;
using Shouldly;
using System;

namespace Explorer.Payments.Tests.Unit
{
    [Collection("Sequential")]
    public class CoinsBundlePurchaseUnitTests
    {
        [Fact]
        public void Creates_purchase_with_valid_data()
        {
            var purchase = new CoinsBundlePurchase(1, 1, "Starter Pack", 500, 5.00m, 5.00m, PaymentMethod.CreditCard, "CC-ABC123");

            purchase.TouristId.ShouldBe(1);
            purchase.CoinsBundleId.ShouldBe(1);
            purchase.BundleName.ShouldBe("Starter Pack");
            purchase.CoinsReceived.ShouldBe(500);
            purchase.PricePaid.ShouldBe(5.00m);
            purchase.OriginalPrice.ShouldBe(5.00m);
            purchase.PaymentMethod.ShouldBe(PaymentMethod.CreditCard);
            purchase.TransactionId.ShouldBe("CC-ABC123");
            purchase.PurchaseDate.ShouldBeGreaterThan(DateTime.MinValue);
        }

        [Fact]
        public void Create_fails_with_invalid_tourist_id()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundlePurchase(0, 1, "Bundle", 500, 5.00m, 5.00m, PaymentMethod.CreditCard, "TX-123")
            ).Message.ShouldContain("Invalid tourist id");
        }

        [Fact]
        public void Create_fails_with_invalid_bundle_id()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundlePurchase(1, 0, "Bundle", 500, 5.00m, 5.00m, PaymentMethod.CreditCard, "TX-123")
            ).Message.ShouldContain("Invalid bundle id");
        }

        [Fact]
        public void Create_fails_with_zero_coins()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundlePurchase(1, 1, "Bundle", 0, 5.00m, 5.00m, PaymentMethod.CreditCard, "TX-123")
            ).Message.ShouldContain("Coins received must be positive");
        }

        [Fact]
        public void Create_fails_with_zero_price()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundlePurchase(1, 1, "Bundle", 500, 0m, 5.00m, PaymentMethod.CreditCard, "TX-123")
            ).Message.ShouldContain("Price paid must be positive");
        }
    }
}
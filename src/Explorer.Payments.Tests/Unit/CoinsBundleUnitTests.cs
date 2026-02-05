using Explorer.Payments.Core.Domain;
using Shouldly;
using System;

namespace Explorer.Payments.Tests.Unit
{
    [Collection("Sequential")]
    public class CoinsBundleUnitTests
    {
        [Fact]
        public void Creates_coins_bundle_with_valid_data()
        {
            var bundle = new CoinsBundle("Starter Pack", "Perfect for beginners", 500, 0, 5.00m, "bundle-1.png", 1);

            bundle.Name.ShouldBe("Starter Pack");
            bundle.Description.ShouldBe("Perfect for beginners");
            bundle.CoinsAmount.ShouldBe(500);
            bundle.BonusCoins.ShouldBe(0);
            bundle.Price.ShouldBe(5.00m);
            bundle.ImageUrl.ShouldBe("bundle-1.png");
            bundle.DisplayOrder.ShouldBe(1);
        }

        [Fact]
        public void GetTotalCoins_returns_correct_sum()
        {
            var bundle = new CoinsBundle("Pro Pack", "Best value", 1000, 200, 10.00m, "bundle-2.png", 2);

            bundle.GetTotalCoins().ShouldBe(1200);
        }

        [Fact]
        public void Create_fails_with_empty_name()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundle("", "Description", 500, 0, 5.00m, "image.png", 1)
            ).Message.ShouldContain("Name cannot be empty");
        }

        [Fact]
        public void Create_fails_with_zero_coins()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundle("Bundle", "Description", 0, 0, 5.00m, "image.png", 1)
            ).Message.ShouldContain("Coins amount must be positive");
        }

        [Fact]
        public void Create_fails_with_negative_bonus()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundle("Bundle", "Description", 500, -10, 5.00m, "image.png", 1)
            ).Message.ShouldContain("Bonus coins cannot be negative");
        }

        [Fact]
        public void Create_fails_with_zero_price()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundle("Bundle", "Description", 500, 0, 0m, "image.png", 1)
            ).Message.ShouldContain("Price must be positive");
        }

        [Fact]
        public void Create_fails_with_negative_price()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundle("Bundle", "Description", 500, 0, -5.00m, "image.png", 1)
            ).Message.ShouldContain("Price must be positive");
        }
    }
}
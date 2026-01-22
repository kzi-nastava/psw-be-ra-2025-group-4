using Explorer.Payments.Core.Domain;
using Shouldly;
using System;

namespace Explorer.Payments.Tests.Unit
{
    [Collection("Sequential")]
    public class CoinsBundleSaleUnitTests
    {
        [Fact]
        public void Creates_sale_with_valid_data()
        {
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(7);
            var sale = new CoinsBundleSale(1, 20m, startDate, endDate);

            sale.CoinsBundleId.ShouldBe(1);
            sale.DiscountPercentage.ShouldBe(20m);
            sale.StartDate.ShouldBe(startDate);
            sale.EndDate.ShouldBe(endDate);
            sale.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void Create_fails_with_invalid_bundle_id()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundleSale(0, 20m, DateTime.UtcNow, DateTime.UtcNow.AddDays(7))
            ).Message.ShouldContain("Invalid bundle id");
        }

        [Fact]
        public void Create_fails_with_zero_discount()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundleSale(1, 0m, DateTime.UtcNow, DateTime.UtcNow.AddDays(7))
            ).Message.ShouldContain("Discount must be between 0 and 100");
        }

        [Fact]
        public void Create_fails_with_discount_over_100()
        {
            Should.Throw<ArgumentException>(
                () => new CoinsBundleSale(1, 150m, DateTime.UtcNow, DateTime.UtcNow.AddDays(7))
            ).Message.ShouldContain("Discount must be between 0 and 100");
        }

        [Fact]
        public void Create_fails_with_end_date_before_start_date()
        {
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(-1);

            Should.Throw<ArgumentException>(
                () => new CoinsBundleSale(1, 20m, startDate, endDate)
            ).Message.ShouldContain("End date must be after start date");
        }

        [Fact]
        public void Deactivate_sets_IsActive_to_false()
        {
            var sale = new CoinsBundleSale(1, 20m, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

            sale.Deactivate();

            sale.IsActive.ShouldBeFalse();
        }

        [Fact]
        public void IsCurrentlyActive_returns_true_for_active_sale_in_date_range()
        {
            var sale = new CoinsBundleSale(1, 20m, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            sale.IsCurrentlyActive().ShouldBeTrue();
        }

        [Fact]
        public void IsCurrentlyActive_returns_false_for_deactivated_sale()
        {
            var sale = new CoinsBundleSale(1, 20m, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
            sale.Deactivate();

            sale.IsCurrentlyActive().ShouldBeFalse();
        }

        [Fact]
        public void IsCurrentlyActive_returns_false_for_future_sale()
        {
            var sale = new CoinsBundleSale(1, 20m, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(7));

            sale.IsCurrentlyActive().ShouldBeFalse();
        }

        [Fact]
        public void IsCurrentlyActive_returns_false_for_expired_sale()
        {
            var sale = new CoinsBundleSale(1, 20m, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(-1));

            sale.IsCurrentlyActive().ShouldBeFalse();
        }

        [Fact]
        public void CalculateDiscountedPrice_applies_discount_correctly()
        {
            var sale = new CoinsBundleSale(1, 20m, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            var discounted = sale.CalculateDiscountedPrice(100m);

            discounted.ShouldBe(80m);
        }

        [Fact]
        public void CalculateDiscountedPrice_returns_original_for_inactive_sale()
        {
            var sale = new CoinsBundleSale(1, 20m, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(7));

            var discounted = sale.CalculateDiscountedPrice(100m);

            discounted.ShouldBe(100m);
        }
    }
}
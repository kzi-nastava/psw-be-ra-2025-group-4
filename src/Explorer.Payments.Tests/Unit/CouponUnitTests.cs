using Explorer.Payments.Core.Domain;
using Shouldly;
using System;

namespace Explorer.Payments.Tests.Unit
{
    [Collection("Sequential")]
    public class CouponUnitTests
    {
        [Fact]
        public void Creates_coupon_with_valid_data()
        {
            var coupon = new Coupon("ABC12345", 20, 1, DateTime.UtcNow.AddDays(30), 1);

            coupon.Code.ShouldBe("ABC12345");
            coupon.DiscountPercentage.ShouldBe(20);
            coupon.AuthorId.ShouldBe(1);
            coupon.TourId.ShouldBe(1);
            coupon.IsUsed.ShouldBeFalse();
            coupon.ExpirationDate.ShouldNotBeNull();
        }

        [Fact]
        public void Creates_coupon_for_all_author_tours()
        {
            var coupon = new Coupon("XYZ78901", 15, 2, null, null);

            coupon.Code.ShouldBe("XYZ78901");
            coupon.AuthorId.ShouldBe(2);
            coupon.TourId.ShouldBeNull();
            coupon.ExpirationDate.ShouldBeNull();
        }

        [Fact]
        public void Create_fails_with_invalid_code_length()
        {
            Should.Throw<BuildingBlocks.Core.Exceptions.EntityValidationException>(
                () => new Coupon("ABC", 20, 1)
            ).Message.ShouldBe("Coupon code must be exactly 8 characters.");
        }

        [Fact]
        public void Create_fails_with_empty_code()
        {
            Should.Throw<BuildingBlocks.Core.Exceptions.EntityValidationException>(
                () => new Coupon("", 20, 1)
            ).Message.ShouldBe("Coupon code must be exactly 8 characters.");
        }

        [Fact]
        public void Create_fails_with_zero_discount()
        {
            Should.Throw<BuildingBlocks.Core.Exceptions.EntityValidationException>(
                () => new Coupon("ABC12345", 0, 1)
            ).Message.ShouldBe("Discount percentage must be between 1 and 100.");
        }

        [Fact]
        public void Create_fails_with_negative_discount()
        {
            Should.Throw<BuildingBlocks.Core.Exceptions.EntityValidationException>(
                () => new Coupon("ABC12345", -10, 1)
            ).Message.ShouldBe("Discount percentage must be between 1 and 100.");
        }

        [Fact]
        public void Create_fails_with_discount_over_100()
        {
            Should.Throw<BuildingBlocks.Core.Exceptions.EntityValidationException>(
                () => new Coupon("ABC12345", 150, 1)
            ).Message.ShouldBe("Discount percentage must be between 1 and 100.");
        }

        [Fact]
        public void Create_fails_with_invalid_author_id()
        {
            Should.Throw<BuildingBlocks.Core.Exceptions.EntityValidationException>(
                () => new Coupon("ABC12345", 20, 0)
            ).Message.ShouldBe("Invalid author id.");
        }

        [Fact]
        public void Updates_coupon_successfully()
        {
            var coupon = new Coupon("ABC12345", 20, 1, DateTime.UtcNow.AddDays(30), 1);
            var newExpiration = DateTime.UtcNow.AddDays(60);

            coupon.Update(30, newExpiration, 2);

            coupon.DiscountPercentage.ShouldBe(30);
            coupon.ExpirationDate.ShouldBe(newExpiration);
            coupon.TourId.ShouldBe(2);
        }

        [Fact]
        public void Update_fails_for_used_coupon()
        {
            var coupon = new Coupon("ABC12345", 20, 1, DateTime.UtcNow.AddDays(30), 1);
            coupon.MarkAsUsed(10);

            Should.Throw<InvalidOperationException>(
                () => coupon.Update(30, DateTime.UtcNow.AddDays(60), 2)
            ).Message.ShouldBe("Cannot update a used coupon.");
        }

        [Fact]
        public void Marks_coupon_as_used_successfully()
        {
            var coupon = new Coupon("ABC12345", 20, 1, DateTime.UtcNow.AddDays(30), 1);

            coupon.MarkAsUsed(10);

            coupon.IsUsed.ShouldBeTrue();
            coupon.UsedByTouristId.ShouldBe(10);
            coupon.UsedAt.ShouldNotBeNull();
        }

        [Fact]
        public void MarkAsUsed_fails_for_already_used_coupon()
        {
            var coupon = new Coupon("ABC12345", 20, 1, DateTime.UtcNow.AddDays(30), 1);
            coupon.MarkAsUsed(10);

            Should.Throw<InvalidOperationException>(
                () => coupon.MarkAsUsed(11)
            ).Message.ShouldBe("Coupon has already been used.");
        }

        [Fact]
        public void MarkAsUsed_fails_for_expired_coupon()
        {
            var coupon = new Coupon("ABC12345", 20, 1, DateTime.UtcNow.AddDays(-1), 1);

            Should.Throw<InvalidOperationException>(
                () => coupon.MarkAsUsed(10)
            ).Message.ShouldBe("Coupon has expired.");
        }

        [Fact]
        public void IsValid_returns_true_for_unused_non_expired_coupon()
        {
            var coupon = new Coupon("ABC12345", 20, 1, DateTime.UtcNow.AddDays(30), 1);

            coupon.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void IsValid_returns_false_for_used_coupon()
        {
            var coupon = new Coupon("ABC12345", 20, 1, DateTime.UtcNow.AddDays(30), 1);
            coupon.MarkAsUsed(10);

            coupon.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void IsValid_returns_false_for_expired_coupon()
        {
            var coupon = new Coupon("ABC12345", 20, 1, DateTime.UtcNow.AddDays(-1), 1);

            coupon.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void IsValid_returns_true_for_coupon_without_expiration()
        {
            var coupon = new Coupon("ABC12345", 20, 1, null, 1);

            coupon.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void GenerateCode_creates_8_character_code()
        {
            var code = Coupon.GenerateCode();

            code.ShouldNotBeNullOrEmpty();
            code.Length.ShouldBe(8);
        }

        [Fact]
        public void GenerateCode_creates_unique_codes()
        {
            var codes = new System.Collections.Generic.HashSet<string>();

            for (int i = 0; i < 100; i++)
            {
                codes.Add(Coupon.GenerateCode());
            }

            codes.Count.ShouldBeGreaterThan(95);
        }
    }
}
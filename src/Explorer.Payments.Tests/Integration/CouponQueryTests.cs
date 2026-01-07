using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Payments.Tests.Integration
{
    [Collection("Sequential")]
    public class CouponQueryTests : BasePaymentsIntegrationTest
    {
        public CouponQueryTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_coupon_by_id()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAuthorController(scope, "-11");

            var result = ((ObjectResult)controller.GetById(-1).Result)?.Value as CouponResponseDto;

            result.ShouldNotBeNull();
            result.Id.ShouldBe(-1);
            result.Code.ShouldBe("TESTCODE");
            result.DiscountPercentage.ShouldBe(20);
            result.AuthorId.ShouldBe(-11);
            result.TourId.ShouldBe(-2);
            result.IsUsed.ShouldBeFalse();
        }

        [Fact]
        public void GetById_fails_for_invalid_id()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICouponService>();

            Should.Throw<NotFoundException>(
                () => service.GetById(-9999)
            );
        }

        [Fact]
        public void Retrieves_all_coupons_for_author()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAuthorController(scope, "-11");

            var result = ((ObjectResult)controller.GetMyCoupons().Result)?.Value as List<CouponResponseDto>;

            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);
            result.All(c => c.AuthorId == -11).ShouldBeTrue();
        }

        [Fact]
        public void Retrieves_only_valid_coupons_in_list()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAuthorController(scope, "-11");

            var result = ((ObjectResult)controller.GetMyCoupons().Result)?.Value as List<CouponResponseDto>;

            result.ShouldNotBeNull();

            foreach (var coupon in result)
            {
                if (coupon.IsUsed || (coupon.ExpirationDate.HasValue && coupon.ExpirationDate.Value < DateTime.UtcNow))
                {
                    coupon.IsValid.ShouldBeFalse();
                }
                else
                {
                    coupon.IsValid.ShouldBeTrue();
                }
            }
        }

        [Fact]
        public void Validates_coupon_for_specific_tour_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var dto = new CouponValidationDto
            {
                Code = "TESTCODE",
                TouristId = -1,
                TourIds = new List<int> { -2 }
            };

            var result = ((ObjectResult)controller.ValidateCoupon(dto).Result)?.Value as CouponValidationResultDto;

            result.ShouldNotBeNull();
            result.IsValid.ShouldBeTrue();
            result.ApplicableTourId.ShouldBe(-2);
            result.DiscountPercentage.ShouldBe(20);
            result.DiscountAmount.ShouldBeGreaterThan(0);
            result.Message.ShouldContain("valid");
        }

        [Fact]
        public void Validate_fails_for_invalid_code()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var dto = new CouponValidationDto
            {
                Code = "INVALID1",
                TouristId = -1,
                TourIds = new List<int> { -2 }
            };

            var result = ((ObjectResult)controller.ValidateCoupon(dto).Result)?.Value as CouponValidationResultDto;

            result.ShouldNotBeNull();
            result.IsValid.ShouldBeFalse();
            result.Message.ShouldBe("Invalid coupon code.");
        }

        [Fact]
        public void Validate_fails_for_expired_coupon()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var dto = new CouponValidationDto
            {
                Code = "EXPIRED1",
                TouristId = -1,
                TourIds = new List<int> { -2 }
            };

            var result = ((ObjectResult)controller.ValidateCoupon(dto).Result)?.Value as CouponValidationResultDto;

            result.ShouldNotBeNull();
            result.IsValid.ShouldBeFalse();
            result.Message.ShouldBe("Coupon has expired.");
        }

        [Fact]
        public void Validate_fails_for_used_coupon()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateTouristController(scope, "-1");

            var dto = new CouponValidationDto
            {
                Code = "USED0001",
                TouristId = -1,
                TourIds = new List<int> { -2 }
            };

            var result = ((ObjectResult)controller.ValidateCoupon(dto).Result)?.Value as CouponValidationResultDto;

            result.ShouldNotBeNull();
            result.IsValid.ShouldBeFalse();
            result.Message.ShouldBe("Coupon has already been used.");
        }

        private static CouponController CreateAuthorController(IServiceScope scope, string authorId)
        {
            return new CouponController(
                scope.ServiceProvider.GetRequiredService<ICouponService>(),
                scope.ServiceProvider.GetRequiredService<ITourService>())
            {
                ControllerContext = BuildContext(authorId)
            };
        }

        private static CouponTouristController CreateTouristController(IServiceScope scope, string touristId)
        {
            return new CouponTouristController(
                scope.ServiceProvider.GetRequiredService<ICouponService>(),
                scope.ServiceProvider.GetRequiredService<ITourService>())
            {
                ControllerContext = BuildContext(touristId)
            };
        }
    }
}
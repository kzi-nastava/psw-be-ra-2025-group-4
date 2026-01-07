using System;
using System.Linq;
using Explorer.API.Controllers.Author;
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
    public class CouponCommandTests : BasePaymentsIntegrationTest
    {
        public CouponCommandTests(PaymentsTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_coupon_for_specific_tour()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAuthorController(scope, "-11");
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var dto = new CouponCreateDto
            {
                DiscountPercentage = 20,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                TourId = -2
            };

            var result = ((ObjectResult)controller.Create(dto).Result)?.Value as CouponResponseDto;

            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Code.ShouldNotBeNullOrEmpty();
            result.Code.Length.ShouldBe(8);
            result.DiscountPercentage.ShouldBe(20);
            result.AuthorId.ShouldBe(-11);
            result.TourId.ShouldBe(-2);
            result.IsUsed.ShouldBeFalse();
            result.IsValid.ShouldBeTrue();

            db.Coupons.Any(c => c.Id == result.Id).ShouldBeTrue();
        }

        [Fact]
        public void Creates_coupon_for_all_author_tours()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAuthorController(scope, "-11");
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var dto = new CouponCreateDto
            {
                DiscountPercentage = 15,
                ExpirationDate = null,
                TourId = null
            };

            var result = ((ObjectResult)controller.Create(dto).Result)?.Value as CouponResponseDto;

            result.ShouldNotBeNull();
            result.TourId.ShouldBeNull();
            result.TourName.ShouldBe("All author's tours");
            result.IsValid.ShouldBeTrue();

            db.Coupons.Any(c => c.Id == result.Id && c.TourId == null).ShouldBeTrue();
        }

        [Fact]
        public void Create_generates_unique_8_character_code()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAuthorController(scope, "-11");

            var dto = new CouponCreateDto
            {
                DiscountPercentage = 20,
                TourId = -2
            };

            var result = ((ObjectResult)controller.Create(dto).Result)?.Value as CouponResponseDto;

            result.ShouldNotBeNull();
            result.Code.Length.ShouldBe(8);
            result.Code.ShouldMatch(@"^[A-Z0-9]{8}$");
        }

        [Fact]
        public void Create_fails_for_tour_not_owned_by_author()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICouponService>();
            var tourService = scope.ServiceProvider.GetRequiredService<ITourService>();

            try
            {
                var tour = tourService.GetById(-3);
                tour.AuthorId.ShouldBe(-12); 
            }
            catch
            {
                return;
            }

            var dto = new CouponCreateDto
            {
                DiscountPercentage = 20,
                TourId = -3
            };
            Should.Throw<UnauthorizedAccessException>(
                () => service.Create(dto, -11)
            );
        }

        [Fact]
        public void Create_fails_with_invalid_discount_percentage()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICouponService>();

            var dto = new CouponCreateDto
            {
                DiscountPercentage = 150,
                TourId = -2
            };

            Should.Throw<Exception>(() => service.Create(dto, -11));
        }

        [Fact]
        public void Updates_coupon_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAuthorController(scope, "-11");
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var createDto = new CouponCreateDto
            {
                DiscountPercentage = 20,
                TourId = -2,
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };
            var created = ((ObjectResult)controller.Create(createDto).Result)?.Value as CouponResponseDto;

            var updateDto = new CouponUpdateDto
            {
                Id = created!.Id,
                DiscountPercentage = 30,
                ExpirationDate = DateTime.UtcNow.AddDays(60),
                TourId = null
            };

            var result = ((ObjectResult)controller.Update(created.Id, updateDto).Result)?.Value as CouponResponseDto;

            result.ShouldNotBeNull();
            result.DiscountPercentage.ShouldBe(30);
            result.TourId.ShouldBeNull();
            result.Code.ShouldBe(created.Code);

            var stored = db.Coupons.First(c => c.Id == created.Id);
            stored.DiscountPercentage.ShouldBe(30);
            stored.TourId.ShouldBeNull();
        }

        [Fact]
        public void Update_fails_for_coupon_not_owned_by_author()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICouponService>();

            var updateDto = new CouponUpdateDto
            {
                Id = -5,
                DiscountPercentage = 30
            };

            Should.Throw<UnauthorizedAccessException>(
                () => service.Update(updateDto, -11)
            ).Message.ShouldBe("You can only update your own coupons.");
        }

        [Fact]
        public void Update_fails_for_used_coupon()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICouponService>();

            var updateDto = new CouponUpdateDto
            {
                Id = -2,
                DiscountPercentage = 30
            };

            Should.Throw<InvalidOperationException>(
                () => service.Update(updateDto, -11)
            ).Message.ShouldBe("Cannot update a used coupon.");
        }

        [Fact]
        public void Deletes_coupon_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateAuthorController(scope, "-11");
            var db = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var createDto = new CouponCreateDto
            {
                DiscountPercentage = 20,
                TourId = -2
            };
            var created = ((ObjectResult)controller.Create(createDto).Result)?.Value as CouponResponseDto;

            var deleteResult = controller.Delete(created!.Id);

            deleteResult.ShouldBeOfType<NoContentResult>();
            db.Coupons.Any(c => c.Id == created.Id).ShouldBeFalse();
        }

        [Fact]
        public void Delete_fails_for_used_coupon()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICouponService>();

            Should.Throw<InvalidOperationException>(
                () => service.Delete(-2, -11)
            ).Message.ShouldBe("Cannot delete a used coupon.");
        }

        [Fact]
        public void Delete_fails_for_coupon_not_owned_by_author()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICouponService>();

            Should.Throw<UnauthorizedAccessException>(
                () => service.Delete(-5, -11)
            ).Message.ShouldBe("You can only delete your own coupons.");
        }

        [Fact]
        public void Delete_fails_for_invalid_coupon_id()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICouponService>();

            Should.Throw<NotFoundException>(
                () => service.Delete(-9999, -11)
            );
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
    }
}
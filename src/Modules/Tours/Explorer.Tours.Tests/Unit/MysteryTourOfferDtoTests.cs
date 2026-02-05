using System;
using Explorer.Tours.API.Dtos;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class MysteryTourOfferDtoTests
    {
        [Fact]
        public void Can_set_and_get_properties()
        {
            var now = DateTime.UtcNow;

            var dto = new MysteryTourOfferDto
            {
                Id = Guid.NewGuid(),
                TouristId = 1,
                TourId = 10,
                TourName = "Tour X",
                OriginalPrice = 200m,
                DiscountPercent = 20,
                DiscountedPrice = 160m,
                ExpiresAt = now.AddMinutes(10),
                Redeemed = false
            };

            dto.Id.ShouldNotBe(Guid.Empty);
            dto.TouristId.ShouldBe(1);
            dto.TourId.ShouldBe(10);
            dto.TourName.ShouldBe("Tour X");
            dto.OriginalPrice.ShouldBe(200m);
            dto.DiscountPercent.ShouldBe(20);
            dto.DiscountedPrice.ShouldBe(160m);
            dto.ExpiresAt.ShouldBe(now.AddMinutes(10));
            dto.Redeemed.ShouldBeFalse();
        }

        [Fact]
        public void Default_tour_name_is_empty_string()
        {
            var dto = new MysteryTourOfferDto();
            dto.TourName.ShouldBe("");
        }
    }
}
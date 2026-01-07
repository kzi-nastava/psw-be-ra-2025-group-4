using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/coupons")]
    [ApiController]
    public class CouponTouristController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private readonly ITourService _tourService;

        public CouponTouristController(ICouponService couponService, ITourService tourService)
        {
            _couponService = couponService;
            _tourService = tourService;
        }

        [HttpPost("validate")]
        public ActionResult<CouponValidationResultDto> ValidateCoupon([FromBody] CouponValidationDto validationDto)
        {
            var touristId = int.Parse(User.Claims.First(c => c.Type == "personId").Value); // ✅ Promenjeno
            validationDto.TouristId = touristId;

            var tourInfos = new Dictionary<int, PaymentTourInfoDto>();
            foreach (var tourId in validationDto.TourIds)
            {
                var tour = _tourService.GetById(tourId);
                tourInfos[tourId] = new PaymentTourInfoDto
                {
                    Id = tour.Id,
                    Name = tour.Name,
                    AuthorId = tour.AuthorId,
                    Price = tour.Price
                };
            }

            var result = _couponService.ValidateCoupon(validationDto, tourInfos);
            return Ok(result);
        }
    }
}
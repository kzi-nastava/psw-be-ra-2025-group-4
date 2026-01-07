using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/coupons")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private readonly ITourService _tourService;

        public CouponController(ICouponService couponService, ITourService tourService)
        {
            _couponService = couponService;
            _tourService = tourService;
        }

        [HttpPost]
        public ActionResult<CouponResponseDto> Create([FromBody] CouponCreateDto couponDto)
        {
            var authorId = int.Parse(User.Claims.First(c => c.Type == "personId").Value); 

            if (couponDto.TourId.HasValue)
            {
                var tour = _tourService.GetById(couponDto.TourId.Value);
                if (tour.AuthorId != authorId)
                    return Unauthorized("You can only create coupons for your own tours.");
            }

            var result = _couponService.Create(couponDto, authorId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public ActionResult<CouponResponseDto> Update(int id, [FromBody] CouponUpdateDto couponDto)
        {
            var authorId = int.Parse(User.Claims.First(c => c.Type == "personId").Value); 

            if (couponDto.TourId.HasValue)
            {
                var tour = _tourService.GetById(couponDto.TourId.Value);
                if (tour.AuthorId != authorId)
                    return Unauthorized("You can only create coupons for your own tours.");
            }

            couponDto.Id = id;
            var result = _couponService.Update(couponDto, authorId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var authorId = int.Parse(User.Claims.First(c => c.Type == "personId").Value); 
            _couponService.Delete(id, authorId);
            return NoContent();
        }

        [HttpGet("{id}")]
        public ActionResult<CouponResponseDto> GetById(int id)
        {
            var result = _couponService.GetById(id);
            return Ok(result);
        }

        [HttpGet("my-coupons")]
        public ActionResult<List<CouponResponseDto>> GetMyCoupons()
        {
            var authorId = int.Parse(User.Claims.First(c => c.Type == "personId").Value); 
            var result = _couponService.GetByAuthor(authorId);
            return Ok(result);
        }
    }
}
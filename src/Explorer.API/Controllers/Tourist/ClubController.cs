using System;
using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/clubs")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _clubService;

        public ClubController(IClubService clubService)
        {
            _clubService = clubService;
        }

        private long GetTouristId()
        {
            var id = User.FindFirst("id")?.Value;

            if (id != null) return long.Parse(id);

            var pid = User.FindFirst("personId")?.Value;

            return long.Parse(pid ?? throw new Exception("No user id found"));
        }

        
        [HttpGet]
        public ActionResult<List<ClubDto>> GetAll()
        {
            var clubs = _clubService.GetAll();
            return Ok(clubs);
        }

        
        [HttpGet("mine")]
        public ActionResult<List<ClubDto>> GetMine()
        {
            var clubs = _clubService.GetByOwner(GetTouristId());
            return Ok(clubs);
        }

        
        [HttpPost]
        public ActionResult<ClubDto> Create([FromBody] ClubDto clubDto)
        {
            clubDto.OwnerId = GetTouristId();

            var created = _clubService.Create(clubDto);
            
            return Created(string.Empty, created);
        }

        
        [HttpPut("{id:long}")]
        public ActionResult<ClubDto> Update(long id, [FromBody] ClubDto clubDto)
        {
            clubDto.OwnerId = GetTouristId();

            var updated = _clubService.Update(id, clubDto);
            return Ok(updated);
        }

        
        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            _clubService.Delete(id, GetTouristId());
            return NoContent();
        }
    }
}
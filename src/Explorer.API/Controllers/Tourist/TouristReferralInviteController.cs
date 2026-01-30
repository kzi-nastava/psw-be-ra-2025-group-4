using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/referral-invites")]
    [ApiController]
    public class TouristReferralInviteController : ControllerBase
    {
        private readonly ITouristReferralInviteService _service;
        private long GetTouristId()
        {
            var id = User.FindFirst("id")?.Value;
            if (id != null) return long.Parse(id);

            var pid = User.FindFirst("personId")?.Value;
            return long.Parse(pid ?? throw new Exception("No user id found"));
        }
        public TouristReferralInviteController(ITouristReferralInviteService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<TouristReferralInviteDto> Create()
        {
            var touristId = GetTouristId();
            return Ok(_service.Create(touristId));
        }
    }
}

using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.UseCases.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administration/users")]
    [ApiController]
    public class AdministrationController : ControllerBase
    {
        private readonly IUserAccountService _userAccountService;

        public AdministrationController(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        [HttpGet]
        public ActionResult<PagedResult<UserAccountDto>> GetPaged([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_userAccountService.GetPaged(page, pageSize));
        }

        [HttpPost]
        public ActionResult<UserAccountDto> Create([FromBody] AccountRegistrationDto account)
        {
            return Ok(_userAccountService.CreateUser(account));
        }

        [HttpPut("block/{id}")]
        public IActionResult BlockUser(long id)
        {
            try
            {
                _userAccountService.BlockUser(id);
                return NoContent(); // ili Ok()
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }


    }
}

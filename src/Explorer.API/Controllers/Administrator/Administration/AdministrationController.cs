using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Public.Tourist;
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
        private readonly IWalletService _walletService;

        public AdministrationController(IUserAccountService userAccountService, IWalletService walletService)
        {
            _userAccountService = userAccountService;
            _walletService = walletService;
        }

        [HttpGet]
        public ActionResult<PagedResult<UserAccountDto>> GetPaged([FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = _userAccountService.GetPaged(page, pageSize);
            
            foreach (var account in result.Results)
            {
                if (account.Role == "tourist")
                {
                    try
                    {
                        var wallet = _walletService.GetWallet((int)account.Id);
                        account.Balance = wallet.Balance;
                    }
                    catch
                    {
                        account.Balance = 0;
                    }
                }
            }
            
            return Ok(result);
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

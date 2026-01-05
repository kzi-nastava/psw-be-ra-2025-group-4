using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Payments.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Route("api/users")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IWalletAdministrationService _walletAdministrationService;

    public AuthenticationController(
        IAuthenticationService authenticationService,
        IWalletAdministrationService walletAdministrationService)
    {
        _authenticationService = authenticationService;
        _walletAdministrationService = walletAdministrationService;
    }

    [HttpPost]
    public ActionResult<AuthenticationTokensDto> RegisterTourist([FromBody] AccountRegistrationDto account)
    {
        var result = _authenticationService.RegisterTourist(account);
        
        try
        {
            var touristId = int.Parse(result.Id.ToString());
            _walletAdministrationService.AddBalance(touristId, 0);
        }
        catch
        {
        }
        
        return Ok(result);
    }

    [HttpPost("login")]
    public ActionResult<AuthenticationTokensDto> Login([FromBody] CredentialsDto credentials)
    {
        return Ok(_authenticationService.Login(credentials));
    }
}
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Payments.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Explorer.Payments.API.Public.Tourist;

namespace Explorer.API.Controllers;

[Route("api/users")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IWalletAdministrationService _walletAdministrationService;
    private readonly ITouristReferralInviteService _touristReferralInviteService;

    public AuthenticationController(
        IAuthenticationService authenticationService,
        IWalletAdministrationService walletAdministrationService,
        ITouristReferralInviteService touristReferralInviteService)
    {
        _authenticationService = authenticationService;
        _walletAdministrationService = walletAdministrationService;
        _touristReferralInviteService = touristReferralInviteService;
    }

    [HttpPost]
    public ActionResult<AuthenticationTokensDto> RegisterTourist([FromBody] AccountRegistrationDto account)
    {
        
        var result = _authenticationService.RegisterTourist(account);

        if (!int.TryParse(result.Id.ToString(), out var touristId))
            return StatusCode(500, "Registration succeeded but user id is invalid.");

        _walletAdministrationService.AddBalance(touristId, 0);

        if (!string.IsNullOrWhiteSpace(account.ReferralCode))
        {
            _touristReferralInviteService.ConsumeOnRegistration(account.ReferralCode, touristId);
        }


        return Ok(result);

    }

    [HttpPost("login")]
    public ActionResult<AuthenticationTokensDto> Login([FromBody] CredentialsDto credentials)
    {
        return Ok(_authenticationService.Login(credentials));
    }
}
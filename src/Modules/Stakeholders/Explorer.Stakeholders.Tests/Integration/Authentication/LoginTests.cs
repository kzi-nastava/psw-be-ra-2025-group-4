using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Stakeholders.Tests.Integration.Authentication;

[Collection("Sequential")]
public class LoginTests : BaseStakeholdersIntegrationTest
{
    public LoginTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Successfully_logs_in()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

       
        var registration = new AccountRegistrationDto
        {
            Username = "login.test@example.com",
            Password = "test123",
            Email = "login.test@example.com",
            Name = "Login",
            Surname = "Test"
        };

        var registrationResult = (ObjectResult)controller.RegisterTourist(registration).Result;
        var registrationTokens = registrationResult.Value as AuthenticationTokensDto;
        registrationTokens.ShouldNotBeNull();

      
        var loginSubmission = new CredentialsDto
        {
            Username = "login.test@example.com",
            Password = "test123"
        };

        var loginResult = (ObjectResult)controller.Login(loginSubmission).Result;
        var authenticationResponse = loginResult.Value as AuthenticationTokensDto;

        
        authenticationResponse.ShouldNotBeNull();
        authenticationResponse!.Id.ShouldNotBe(0);  

       
        var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(authenticationResponse.AccessToken);
        var personId = decodedToken.Claims.FirstOrDefault(c => c.Type == "personId");
        personId.ShouldNotBeNull();
    }

    [Fact]
    public void Not_registered_user_fails_login()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "turistaY@gmail.com", Password = "turista1" };

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() => controller.Login(loginSubmission));
    }

    [Fact]
    public void Invalid_password_fails_login()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var loginSubmission = new CredentialsDto { Username = "turista3@gmail.com", Password = "123" };

        // Act & Assert
        Should.Throw<UnauthorizedAccessException>(() => controller.Login(loginSubmission));
    }

    private static AuthenticationController CreateController(IServiceScope scope)
    {
        return new AuthenticationController(scope.ServiceProvider.GetRequiredService<IAuthenticationService>());
    }
}
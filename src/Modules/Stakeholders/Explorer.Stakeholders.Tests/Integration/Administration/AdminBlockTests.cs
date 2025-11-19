using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.API.Controllers.Administrator.Administration;

namespace Explorer.Stakeholders.Tests.Integration.Administration;

[Collection("Sequential")]
public class AdminBlockTests : BaseStakeholdersIntegrationTest
{
    public AdminBlockTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void BlockAuthor_Succeeds()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var userSetup = dbContext.Users.First(u => u.Id == -11);
        userSetup.IsActive = true;
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.BlockUser(-11);

        // Assert
        var actionResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, actionResult.StatusCode);

        // Assert - baza
        dbContext.ChangeTracker.Clear();
        var user = dbContext.Users.First(u => u.Id == -11);
        Assert.False(user.IsActive);
    }

    [Fact]
    public void BlockUser_Twice_ReturnsBadRequestOnSecondAttempt()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        var userId = -11;

        // Act - prvi put blokiranje
        var firstResult = controller.BlockUser(userId);
        var firstActionResult = Assert.IsType<NoContentResult>(firstResult);
        Assert.Equal(204, firstActionResult.StatusCode);

        // Act - drugi put blokiranje
        var secondResult = controller.BlockUser(userId);
        var secondActionResult = Assert.IsType<BadRequestObjectResult>(secondResult);
        Assert.Equal(400, secondActionResult.StatusCode);

        // Assert - baza
        dbContext.ChangeTracker.Clear();
        var user = dbContext.Users.First(u => u.Id == userId);
        Assert.False(user.IsActive);
    }

    [Fact]
    public void BlockAdmin_Fails_BecauseAdminCannotBeBlocked()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var adminId = -1; 

        // Act
        var result = controller.BlockUser(adminId);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }


    private static AdministrationController CreateController(IServiceScope scope)
    {
        return new AdministrationController(scope.ServiceProvider.GetRequiredService<IUserAccountService>());
    }
}

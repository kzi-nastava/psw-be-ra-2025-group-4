using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.API.Controllers.Administrator.Administration;
using Explorer.BuildingBlocks.Tests;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Tests.Integration.Administration;

[Collection("Sequential")]
public class AdminBlockTests : BaseStakeholdersIntegrationTest
{
    public AdminBlockTests(StakeholdersTestFactory factory) : base(factory) { }

    /*[Fact]
    public void BlockAuthor_Succeeds()
    {
        ResetAndSeedDatabase();
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        // Kreiraj testnog korisnika
        var testUser = new User(
            username: "new01",
            password: "pass123",
            role: UserRole.Author,
            isActive: true
        );
        dbContext.Users.Add(testUser);
        dbContext.SaveChanges();

        var controller = CreateController(scope);

        // Act
        var result = controller.BlockUser(testUser.Id); // koristi ID koji je baza dodelila

        // Assert - response
        var actionResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, actionResult.StatusCode);

        // Assert - baza
        dbContext.ChangeTracker.Clear();
        var userInDb = dbContext.Users.First(u => u.Id == testUser.Id);
        Assert.False(userInDb.IsActive); // mora biti blokiran
    }



    [Fact]
    public void BlockUser_Twice_ReturnsBadRequestOnSecondAttempt()
    {
        ResetAndSeedDatabase();
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        // Kreiraj testnog korisnika
        var testUser = new User(
            username: "new02",
            password: "pass123",
            role: UserRole.Author,
            isActive: true
        );
        dbContext.Users.Add(testUser);
        dbContext.SaveChanges();

        var userId = testUser.Id; // koristimo ID koji je baza dodelila

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
        var userInDb = dbContext.Users.First(u => u.Id == userId);
        Assert.False(userInDb.IsActive);
    }


    [Fact]
    public void BlockAdmin_Fails_BecauseAdminCannotBeBlocked()
    {
        ResetAndSeedDatabase();
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        // Kreiraj admin korisnika
        var adminUser = new User(
            username: "admin1",
            password: "adminpass",
            role: UserRole.Administrator,
            isActive: true
        );
        dbContext.Users.Add(adminUser);
        dbContext.SaveChanges();

        var adminId = adminUser.Id; // ID koji je baza dodelila

        // Act
        var result = controller.BlockUser(adminId);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }*/



    private static AdministrationController CreateController(IServiceScope scope)
    {
        return new AdministrationController(scope.ServiceProvider.GetRequiredService<IUserAccountService>());
    }
}

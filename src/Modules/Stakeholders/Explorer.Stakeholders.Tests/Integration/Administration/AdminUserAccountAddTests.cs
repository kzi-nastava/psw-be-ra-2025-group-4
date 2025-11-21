using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Stakeholders.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using System;

namespace Explorer.Stakeholders.Tests.Integration.Administration;

[Collection("Sequential")]
public class AdminUserAccountAddTests : BaseStakeholdersIntegrationTest
{
    public AdminUserAccountAddTests(StakeholdersTestFactory factory) : base(factory) { }

    /*[Fact]
    public void CreateUser_Succeeds()
    {
        ResetAndSeedDatabase();
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var controller = CreateController(scope);

        var dto = new AccountRegistrationDto
        {
            Username = "newuser123",
            Password = "pass123",
            Email = "newuser@mail.com",
            Name = "Pera",
            Surname = "Perić",
            Role = "Author"
        };

        // Act
        var result = controller.Create(dto);

        // Assert – HTTP odgovor
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, ok.StatusCode);

        var createdUser = Assert.IsType<UserAccountDto>(ok.Value);

        // DTO provere
        Assert.Equal(dto.Username, createdUser.Username);
        Assert.Equal("author", createdUser.Role);
        Assert.True(createdUser.IsActive);
        Assert.Equal(dto.Email, createdUser.Email);

        // Assert – database stanje
        dbContext.ChangeTracker.Clear();

        var user = dbContext.Users.FirstOrDefault(u => u.Username == dto.Username);
        Assert.NotNull(user);
        Assert.True(user.IsActive);

        // Provera role-a
        Assert.Equal(UserRole.Author, user.Role);

        var person = dbContext.People.FirstOrDefault(p => p.Email == dto.Email);
        Assert.NotNull(person);
        Assert.Equal(dto.Name, person.Name);
        Assert.Equal(dto.Surname, person.Surname);
    }

    [Fact]
    public void CreateUser_Twice_ThrowsException()
    {
        ResetAndSeedDatabase();
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUserAccountService>();

        var dto = new AccountRegistrationDto
        {
            Username = "testuser123",
            Password = "pass",
            Email = "mail@mail.com",
            Name = "Pera",
            Surname = "Peric",
            Role = "Author"
        };

        // Act – prvi put mora da radi
        var ok1 = service.CreateUser(dto);
        Assert.NotNull(ok1);

        // Act + Assert – drugi put mora da baci exception
        Assert.Throws<EntityValidationException>(() =>
        {
            service.CreateUser(dto);
        });
    }

    [Fact]
    public void CreateUser_TouristRole_ThrowsException()
    {
        ResetAndSeedDatabase();
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUserAccountService>();

        var dto = new AccountRegistrationDto
        {
            Username = "pera123",
            Password = "pass123",
            Email = "pera@mail.com",
            Name = "Pera",
            Surname = "Peric",
            Role = "Tourist" 
        };

        Assert.Throws<EntityValidationException>(() =>
        {
            service.CreateUser(dto);
        });
    }*/




    private static AdministrationController CreateController(IServiceScope scope)
    {
        return new AdministrationController(scope.ServiceProvider.GetRequiredService<IUserAccountService>());
    }
}

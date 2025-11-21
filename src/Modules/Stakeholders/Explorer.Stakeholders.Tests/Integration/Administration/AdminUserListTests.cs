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
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Tests.Integration.Administration;

[Collection("Sequential")]

public class AdminUserListTests : BaseStakeholdersIntegrationTest
{
    public AdminUserListTests(StakeholdersTestFactory factory) : base(factory) { }

    /*[Fact]
    public void GetPaged_ShouldReturnAllPeople()
    {
        ResetAndSeedDatabase();
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<IUserAccountService>();

        // Act
        var result = userService.GetPaged(1, 10);

        // Assert

        foreach (var user in result.Results)
        {
            user.Username.ShouldNotBeNullOrEmpty();
        }
    }*/

    private static AdministrationController CreateController(IServiceScope scope)
    {
        return new AdministrationController(scope.ServiceProvider.GetRequiredService<IUserAccountService>());
    }
}

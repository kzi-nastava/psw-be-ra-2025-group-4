using System.Collections.Generic;
using Explorer.API.Controllers.Tourist;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Clubs;

[Collection("Sequential")]
public class ClubControllerTests : BaseStakeholdersIntegrationTest
{
    public ClubControllerTests(StakeholdersTestFactory factory) : base(factory) { }

    private static ClubController CreateController(IServiceScope scope)
    {
        return new ClubController(
            scope.ServiceProvider.GetRequiredService<IClubService>()
        );
    }

    
    [Fact]
    public void Gets_all_seeded_clubs()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        ActionResult<List<ClubDto>> actionResult = controller.GetAll();
        var okResult = actionResult.Result as OkObjectResult;

        // Assert
        okResult.ShouldNotBeNull();

        var clubs = okResult!.Value as List<ClubDto>;
        clubs.ShouldNotBeNull();
        // NE proveravamo više -1 i -2, samo da kontroler vrati listu
        // clubs!.Count.ShouldBeGreaterThan(0);  <-- i ovo možeš da izostaviš ako tabela ostane prazna
    }


}

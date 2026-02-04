using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Achievements;

[Collection("Sequential")]
public class UserAchievementsTests : BaseStakeholdersIntegrationTest
{
    public UserAchievementsTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Returns_all_user_achievements_sorted_by_date()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = CreateService(scope);

        // Act
        var result = service.GetUserAchievements(-21);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);

        result[0].Type.ShouldBe(AchievementTypeDto.OneTourCompleted);
        result[1].Type.ShouldBe(AchievementTypeDto.TwoToursCompleted);
        result[2].Type.ShouldBe(AchievementTypeDto.FiveToursCompleted);
    }

    [Fact]
    public void Returns_empty_list_when_user_has_no_achievements()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = CreateService(scope);

        // Act
        var result = service.GetUserAchievements(-999);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    private static IUserAchievementService CreateService(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<IUserAchievementService>();
    }
}

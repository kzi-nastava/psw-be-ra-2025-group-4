using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class UserAchievementService : IUserAchievementService
{
    private readonly IUserAchievementsRepository _repo;

    public UserAchievementService(IUserAchievementsRepository repo)
    {
        _repo = repo;
    }

    public void EvaluateTourAchievements(long userId, int completedTours)
    {
        var achievements = _repo.GetByUserId(userId)
                           ?? new UserAchievements(userId);

        //var completedTours = _tourStats.CompletedCountForUser(userId);

        achievements.GrantTourAchievements(completedTours, DateTime.UtcNow);

        _repo.Save(achievements);
    }
}

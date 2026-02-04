using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class UserAchievementService : IUserAchievementService
{
    private readonly IUserAchievementsRepository _repo;
    private readonly INotificationService _notificationService;

    public UserAchievementService(IUserAchievementsRepository repo, INotificationService notificationService)
    {
        _repo = repo;
        _notificationService = notificationService;
    }

    public void EvaluateTourAchievements(long userId, int completedTours)
    {
        var achievements = _repo.GetByUserId(userId)
                           ?? new UserAchievements(userId);

        //var completedTours = _tourStats.CompletedCountForUser(userId);

        var newAchievements =
            achievements.GrantTourAchievements(completedTours, DateTime.UtcNow);

        _repo.Save(achievements);

        foreach (var achievement in newAchievements)
        {
            _notificationService.SendAchievementNotification(userId);
        }
    }

    public List<UserAchievementDto> GetUserAchievements(long userId)
    {
        var userAchievements = _repo.GetByUserId(userId);
        if (userAchievements == null)
            return new List<UserAchievementDto>();

        return userAchievements.AchievementsReadOnly
            .Select(a => new UserAchievementDto
            {
                Type = a.Type switch
                {
                    AchievementType.OneTourCompleted => AchievementTypeDto.OneTourCompleted,
                    AchievementType.TwoToursCompleted => AchievementTypeDto.TwoToursCompleted,
                    AchievementType.FiveToursCompleted => AchievementTypeDto.FiveToursCompleted,
                    AchievementType.TenToursCompleted => AchievementTypeDto.TenToursCompleted,
                    _ => throw new ArgumentOutOfRangeException()
                },
                EarnedAt = a.EarnedAt
            })
            .OrderBy(a => a.EarnedAt)
            .ToList();
    }

}

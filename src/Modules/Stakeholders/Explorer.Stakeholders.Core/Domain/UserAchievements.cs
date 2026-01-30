using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class UserAchievements : Entity
{
    public long UserId { get; private set; }

    public List<UserAchievement> Achievements { get; private set; } = new();
    
    [NotMapped]
    public IReadOnlyCollection<UserAchievement> AchievementsReadOnly => Achievements.AsReadOnly();

    private UserAchievements() { }

    public UserAchievements(long userId)
    {
        UserId = userId;
    }

    public void GrantTourAchievements(int completedTours, DateTime now)
    {
        GrantIfReached(1, AchievementType.OneTourCompleted, completedTours, now);
        GrantIfReached(2, AchievementType.TwoToursCompleted, completedTours, now);
        GrantIfReached(5, AchievementType.FiveToursCompleted, completedTours, now);
        GrantIfReached(10, AchievementType.TenToursCompleted, completedTours, now);
    }

    private void GrantIfReached(int required, AchievementType type, int completedTours, DateTime now)
    {
        if (completedTours < required) return;
        if (Achievements.Any(a => a.Type == type)) return;

        Achievements.Add(new UserAchievement(type, now));
    }
}
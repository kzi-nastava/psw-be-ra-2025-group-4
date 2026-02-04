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

    public List<AchievementType> GrantTourAchievements(int completedTours, DateTime now)
    {
        var granted = new List<AchievementType>();

        if (GrantIfReached(1, AchievementType.OneTourCompleted, completedTours, now))
            granted.Add(AchievementType.OneTourCompleted);

        if (GrantIfReached(2, AchievementType.TwoToursCompleted, completedTours, now))
            granted.Add(AchievementType.TwoToursCompleted);

        if (GrantIfReached(5, AchievementType.FiveToursCompleted, completedTours, now))
            granted.Add(AchievementType.FiveToursCompleted);

        if (GrantIfReached(10, AchievementType.TenToursCompleted, completedTours, now))
            granted.Add(AchievementType.TenToursCompleted);

        return granted;
    }


    private bool GrantIfReached(int required, AchievementType type, int completedTours, DateTime now)
    {
        if (completedTours < required)
            return false;

        if (Achievements.Any(a => a.Type == type))
            return false;

        Achievements.Add(new UserAchievement(type, now));
        return true;
    }
}
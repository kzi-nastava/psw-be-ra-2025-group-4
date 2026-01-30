using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class UserAchievement
{
    public long Id { get; private set; }   

    public AchievementType Type { get; private set; }
    public DateTime EarnedAt { get; private set; }

    private UserAchievement() { } 

    internal UserAchievement(AchievementType type, DateTime earnedAt)
    {
        Type = type;
        EarnedAt = earnedAt;
    }
}

public enum AchievementType
{
    OneTourCompleted,
    TwoToursCompleted,
    FiveToursCompleted,
    TenToursCompleted
}
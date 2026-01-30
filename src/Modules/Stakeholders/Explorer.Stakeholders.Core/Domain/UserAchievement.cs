using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class UserAchievement : ValueObject
{
    public AchievementType Type { get; }
    public DateTime EarnedAt { get; }

    private UserAchievement() { }

    public UserAchievement(AchievementType type, DateTime earnedAt)
    {
        Type = type;
        EarnedAt = earnedAt;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Type;
    }
}

public enum AchievementType
{
    OneTourCompleted,
    TwoToursCompleted,
    FiveToursCompleted,
    TenToursCompleted
}
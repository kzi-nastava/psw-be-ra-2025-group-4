using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos;

public class UserAchievementDto
{
    public AchievementTypeDto Type { get; set; }
    public DateTime EarnedAt { get; set; }
}

public enum AchievementTypeDto
{
    OneTourCompleted,
    TwoToursCompleted,
    FiveToursCompleted,
    TenToursCompleted
}
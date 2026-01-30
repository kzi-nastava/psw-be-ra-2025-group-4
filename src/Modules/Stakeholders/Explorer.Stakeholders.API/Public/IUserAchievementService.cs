using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IUserAchievementService
{
    void EvaluateTourAchievements(long userId, int completedTours);
    List<UserAchievementDto> GetUserAchievements(long userId);
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public;

public interface IUserAchievementService
{
    void EvaluateTourAchievements(long userId, int completedTours);
}


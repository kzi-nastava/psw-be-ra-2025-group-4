using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IUserAchievementsRepository
{
    UserAchievements? GetByUserId(long userId);
    void Save(UserAchievements achievements);
}

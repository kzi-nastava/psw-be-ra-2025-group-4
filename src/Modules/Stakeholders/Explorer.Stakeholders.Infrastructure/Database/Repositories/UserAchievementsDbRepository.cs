using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class UserAchievementsRepository : IUserAchievementsRepository
    {
        private readonly StakeholdersContext _context;

        public UserAchievementsRepository(StakeholdersContext context)
        {
            _context = context;
        }

        public UserAchievements? GetByUserId(long userId)
        {
            return _context.UserAchievements
                .Include(ua => ua.Achievements) // ako koristiš navigaciona svojstva
                .FirstOrDefault(ua => ua.UserId == userId);
        }

        public void Save(UserAchievements achievements)
        {
            var existing = _context.UserAchievements
                .Include(ua => ua.Achievements)
                .FirstOrDefault(ua => ua.UserId == achievements.UserId);

            if (existing == null)
            {
                _context.UserAchievements.Add(achievements);
            }
            else
            {
                // NIŠTA ne briši, samo pusti tracking
                // achievements je već TRACKED jer dolazi iz context-a
            }

            _context.SaveChanges();
        }

    }
}

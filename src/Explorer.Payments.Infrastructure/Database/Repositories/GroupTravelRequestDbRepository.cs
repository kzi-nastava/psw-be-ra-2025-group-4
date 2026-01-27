using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class GroupTravelRequestDbRepository : IGroupTravelRequestRepository
    {
        private readonly PaymentsContext _dbContext;
        private readonly DbSet<GroupTravelRequest> _dbSet;

        public GroupTravelRequestDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<GroupTravelRequest>();
        }

        public GroupTravelRequest? GetById(int id)
        {
            return _dbSet
                .Include(r => r.Participants)
                .FirstOrDefault(r => r.Id == id);
        }

        public List<GroupTravelRequest> GetByOrganizerId(int organizerId)
        {
            return _dbSet
                .Include(r => r.Participants)
                .Where(r => r.OrganizerId == organizerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public List<GroupTravelRequest> GetByParticipantId(int participantId)
        {
            return _dbSet
                .Include(r => r.Participants)
                .Where(r => r.Participants.Any(p => p.TouristId == participantId))
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public GroupTravelRequest? GetPendingByParticipantAndTour(int participantId, int tourId)
        {
            return _dbSet
                .Include(r => r.Participants)
                .FirstOrDefault(r => r.TourId == tourId && 
                    r.Participants.Any(p => p.TouristId == participantId) &&
                    r.Status == GroupTravelStatus.Pending);
        }

        public GroupTravelRequest Create(GroupTravelRequest request)
        {
            _dbSet.Add(request);
            _dbContext.SaveChanges();
            return request;
        }

        public GroupTravelRequest Update(GroupTravelRequest request)
        {
            _dbContext.Update(request);
            _dbContext.SaveChanges();
            return request;
        }
    }
}

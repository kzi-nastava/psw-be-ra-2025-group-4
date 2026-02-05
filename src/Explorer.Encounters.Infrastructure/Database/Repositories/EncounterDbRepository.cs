using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterDbRepository : IEncounterRepository
    {
        protected readonly EncountersContext DbContext;
        private readonly DbSet<Encounter> _dbSet;

        public EncounterDbRepository(EncountersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<Encounter>();
        }

        public Encounter Create(Encounter encounter)
        {
            _dbSet.Add(encounter);
            DbContext.SaveChanges();
            return encounter;
        }

        public void Delete(long id)
        {
            var entity = GetById(id);
            if (entity == null)
                throw new NotFoundException("Encounter not found: " + id);

            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public Encounter? GetById(long id)
        {
            var entity = _dbSet
                .FirstOrDefault(t => t.Id == id);

            if (entity == null)
                throw new NotFoundException("Encounter not found: " + id);

            return entity;
        }

        public Encounter? GetQuizById(long id)
        {
            return DbContext.Encounters
                .Include(e => ((QuizEncounter)e).Questions)!
                    .ThenInclude(q => q.Answers)!
                .FirstOrDefault(e => e.Id == id);
        }


        public PagedResult<Encounter> GetPaged(int pageNumber, int pageSize)
        {
            var task = _dbSet.GetPagedById(pageSize, pageNumber);
            task.Wait();  
            return task.Result;
        }

        public IEnumerable<Encounter> GetActive()
        {
            return _dbSet
                .Where(e => e.Status == Core.Domain.EncounterStatus.Active && e.ApprovalStatus == EncounterApprovalStatus.APPROVED)
                .ToList();
        }

        public IEnumerable<Encounter> GetPending()
        {
            return _dbSet
                .Where(e => e.ApprovalStatus == EncounterApprovalStatus.PENDING)
                .ToList();
        }

        public Encounter Update(Encounter encounter)
        {
            try
            {
                DbContext.Update(encounter);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return encounter;
        }
        public List<Encounter> GetByTourPointIds(IEnumerable<int> tourPointIds)
        {
            var encounters = _dbSet
                .Where(e => e.TourPointId.HasValue &&
                            tourPointIds.Contains((int)e.TourPointId.Value) &&
                            e.ApprovalStatus == EncounterApprovalStatus.APPROVED)
                .ToList();

            LoadQuizAggregates(encounters);

            return encounters;
        }

        private void LoadQuizAggregates(List<Encounter> encounters)
        {
            var quizIds = encounters
                .OfType<QuizEncounter>()
                .Select(q => q.Id)
                .ToList();

            if (!quizIds.Any()) return;

            DbContext.QuizEncounters
                .Where(q => quizIds.Contains(q.Id))
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .Load();
        }



        public Encounter? GetByTourPointId(int tourPointId)
        {
            return _dbSet
                .FirstOrDefault(e => e.TourPointId == tourPointId 
                               && e.ApprovalStatus == EncounterApprovalStatus.APPROVED);
        }

        public IEnumerable<Encounter> GetPendingEncounters()
        {
            return _dbSet
                .Where(e => e.ApprovalStatus == EncounterApprovalStatus.PENDING)
                .ToList();
        }

        public IEnumerable<Encounter> GetByTourist(long touristId)
        {
            return _dbSet.Where(e => e.ApprovalStatus == EncounterApprovalStatus.PENDING);
        }
    }
}

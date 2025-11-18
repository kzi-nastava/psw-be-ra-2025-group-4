using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Microsoft.EntityFrameworkCore;
using Explorer.BuildingBlocks.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class DigitalDiaryDbRepository : IDigitalDiaryRepository
    {
        protected readonly BlogContext DbContext;
        private readonly DbSet<DigitalDiary> _dbSet;

        public DigitalDiaryDbRepository(BlogContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<DigitalDiary>();
        }

        public PagedResult<DigitalDiary> GetPagedByTourist(long touristId, int page, int pageSize)
        {
            var query = _dbSet.Where(d => d.TouristId == touristId);
            var task = query.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public DigitalDiary? GetById(long id)
        {
            return _dbSet.Find(id);
        }

        public DigitalDiary Create(DigitalDiary entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public DigitalDiary Update(DigitalDiary entity)
        {
            try
            {
                DbContext.Update(entity);
                DbContext.SaveChanges();
                return entity;
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
        }

        public void Delete(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("Not found: " + id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }
    }
}

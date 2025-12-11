using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class BlogDbRepository : IBlogRepository
    {
        private readonly BlogContext _dbContext;
        private readonly DbSet<BlogPost> _dbSet;

        public BlogDbRepository(BlogContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<BlogPost>();
        }

        public BlogPost Create(BlogPost blog)
        {
            _dbSet.Add(blog);
                _dbContext.SaveChanges();
            return blog;
        }

        public BlogPost Get(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("Blog not found: " + id);
            return entity;
        }

        public IEnumerable<BlogPost> GetByUser(int userId)
        {
            return _dbSet.Where(x => x.UserId == userId).ToList();
        }

        public BlogPost Update(BlogPost blog)
        {
            try
            {
                _dbContext.Update(blog);
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return blog;
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }
        public IEnumerable<BlogPost> GetAll()
        {
            return _dbSet.ToList();
        }

    }
}

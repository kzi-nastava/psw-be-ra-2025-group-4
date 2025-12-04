using System.Collections.Generic;
using System.Linq;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database.Repositories
{
    public class CommentDbRepository : ICommentRepository
    {
        private readonly BlogContext _dbContext;
        private readonly DbSet<Comment> _dbSet;

        public CommentDbRepository(BlogContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<Comment>();
        }

        public IEnumerable<Comment> GetByBlog(int blogId)
        {
            return _dbSet
                .Where(c => c.BlogId == blogId)
                .OrderBy(c => c.CreatedAt)
                .ToList();
        }

        public Comment Get(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) throw new NotFoundException("Comment not found: " + id);
            return entity;
        }

        public Comment Create(Comment comment)
        {
            _dbSet.Add(comment);
            _dbContext.SaveChanges();
            return comment;
        }

        public Comment Update(Comment comment)
        {
            _dbContext.Update(comment);
            _dbContext.SaveChanges();
            return comment;
        }

        public void Delete(int id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            _dbContext.SaveChanges();
        }
    }
}

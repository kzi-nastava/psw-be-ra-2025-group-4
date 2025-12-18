using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

public class CommentDbRepository : ICommentRepository
{
    private readonly BlogContext _dbContext;
    private readonly DbSet<Comment> _dbSet;

    public CommentDbRepository(BlogContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<Comment>();
    }

    public IEnumerable<Comment> GetByBlog(long blogId)
    {
        return _dbSet
            .Where(c => c.BlogId == blogId)
            .OrderBy(c => c.CreatedAt)
            .ToList();
    }

    public Comment Get(long id)
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

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        _dbContext.SaveChanges();
    }

    public int CountByBlog(long blogId)
    {
        return _dbContext.Comments.Count(c => c.BlogId == blogId);
    }


}

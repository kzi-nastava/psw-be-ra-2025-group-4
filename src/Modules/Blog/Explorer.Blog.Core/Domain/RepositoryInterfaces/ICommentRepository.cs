using System.Collections.Generic;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface ICommentRepository
    {
      
        IEnumerable<Comment> GetByBlog(long blogId);
        Comment Get(long id);
        Comment Create(Comment comment);
        Comment Update(Comment comment);
        void Delete(long id);
    }
}

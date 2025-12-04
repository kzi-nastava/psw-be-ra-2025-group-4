using System.Collections.Generic;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface ICommentRepository
    {
        IEnumerable<Comment> GetByBlog(int blogId);
        Comment Get(int id);
        Comment Create(Comment comment);
        Comment Update(Comment comment);
        void Delete(int id);
    }
}

using System.Collections.Generic;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public
{
    public interface ICommentService
    {
        IEnumerable<CommentDto> GetByBlog(long blogId);
        CommentDto Create(long blogId, int userId, CreateUpdateCommentDto dto);
        CommentDto Update(long id, int userId, CreateUpdateCommentDto dto);
        void Delete(long id, int userId);
    }
}

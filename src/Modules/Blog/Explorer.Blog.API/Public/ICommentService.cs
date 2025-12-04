using System.Collections.Generic;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.API.Public
{
    public interface ICommentService
    {
        IEnumerable<CommentDto> GetByBlog(int blogId);
        CommentDto Create(int blogId, int userId, CreateUpdateCommentDto dto);
        CommentDto Update(int id, int userId, CreateUpdateCommentDto dto);
        void Delete(int id, int userId);
    }
}

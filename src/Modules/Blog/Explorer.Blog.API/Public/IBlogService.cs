using Explorer.Blog.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Public
{
    public interface IBlogService
    {
        BlogDto CreateBlog(CreateUpdateBlogDto dto, int userId);
        BlogDto UpdateBlog(int id, CreateUpdateBlogDto dto, int userId);
        BlogDto Get(int id);
        IEnumerable<BlogDto> GetForUser(int userId);
        void DeleteBlog(int id, int userId);
    }
}

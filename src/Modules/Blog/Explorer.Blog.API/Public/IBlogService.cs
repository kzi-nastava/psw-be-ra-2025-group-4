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

        BlogDto GetForUser(int id, int userId);
        IEnumerable<BlogDto> GetByUser(int userId);

        IEnumerable<BlogDto> GetVisible(int userId);
        void DeleteBlog(int id, int userId);
        void Publish(int id, int userId);
        void Archive(int id, int userId);
    }
}

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
        BlogDto UpdateBlog(long id, CreateUpdateBlogDto dto, int userId);
        BlogDto Get(long id);
        IEnumerable<BlogDto> GetByUser(int userId);
        void DeleteBlog(long id, int userId);
        void Publish(long id, int userId);
        void Archive(long id, int userId);

        BlogDto GetForUser(long id, int userId);
        IEnumerable<BlogDto> GetVisible(int userId);
    }
}

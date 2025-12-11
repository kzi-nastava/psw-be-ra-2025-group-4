using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface IBlogRepository
    {
        BlogPost Get(long id);
        IEnumerable<BlogPost> GetByUser(int userId);
        BlogPost Create(BlogPost blog);
        BlogPost Update(BlogPost blog);
        void Delete(long id);
        IEnumerable<BlogPost> GetAll();
    }
}

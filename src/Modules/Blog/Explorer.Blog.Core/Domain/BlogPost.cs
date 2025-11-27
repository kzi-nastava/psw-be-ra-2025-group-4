using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain
{
    public class BlogPost
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public List<string> Images { get; private set; }
        public int UserId { get; private set; }

        protected BlogPost(){}

        public BlogPost(string title, string description, int userId, List<string>? images = null)
        {
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            UserId = userId;
            Images = images ?? new List<string>();
        }

        public void Update(string title, string description, List<string> images)
        {
            Title = title;
            Description = description;
            Images = images;
        }
    }
}

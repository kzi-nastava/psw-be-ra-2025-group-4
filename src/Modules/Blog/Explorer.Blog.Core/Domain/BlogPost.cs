using System;
using System.Collections.Generic;

namespace Explorer.Blog.Core.Domain
{
    public class BlogPost
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastUpdatedAt { get; private set; }
        public List<string> Images { get; private set; }
        public int UserId { get; private set; }
        public BlogStatus Status { get; private set; }

        protected BlogPost() { }

        public BlogPost(string title, string description, int userId, List<string>? images = null)
        {
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            UserId = userId;
            Images = images ?? new List<string>();
            Status = BlogStatus.Preparation; 
        }

        public void Update(string title, string description, List<string> images)
        {
            if (Status == BlogStatus.Archived)
                throw new InvalidOperationException("Archived blog cannot be modified.");

            if (Status == BlogStatus.Preparation)
            {
                Title = title;
                Description = description;
                Images = images;
                return;
            }

            if (Status == BlogStatus.Published)
            {
                Description = description;
                LastUpdatedAt = DateTime.UtcNow;
                return;
            }
        }

        public void Publish()
        {
            if (Status != BlogStatus.Preparation)
                throw new InvalidOperationException("Only blogs in preparation can be published.");

            Status = BlogStatus.Published;
        }

        public void Archive()
        {
            if (Status != BlogStatus.Published)
                throw new InvalidOperationException("Only published blogs can be archived.");

            Status = BlogStatus.Archived;
        }
    }
}

using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain
{
    public class Comment : Entity
    {
        public long BlogId { get; private set; }
        public int UserId { get; private set; }
        public string Text { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        

        protected Comment() { }

        public Comment(long blogId, int userId, string text)
        {
            BlogId = blogId;
            UserId = userId;
            Text = text;
            CreatedAt = DateTime.UtcNow;
        }

        public Comment(long blogId, int userId, string text, DateTime createdAt)
        {
            BlogId = blogId;
            UserId = userId;
            Text = text;
            CreatedAt = createdAt;
        }

        public void UpdateText(string text, DateTime now)
        {
            if ((now - CreatedAt).TotalMinutes > 15)
                throw new InvalidOperationException("Comment can be edited only within 15 minutes.");

            Text = text;
            LastModifiedAt = now;
        }

        public void EnsureCanBeDeleted(DateTime now)
        {
            if ((now - CreatedAt).TotalMinutes > 15)
                throw new InvalidOperationException("Comment can be deleted only within 15 minutes.");
        }
    }
}

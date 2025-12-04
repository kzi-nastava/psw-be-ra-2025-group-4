using System;
using System.Collections.Generic;

namespace Explorer.Blog.API.Dtos
{
    public class BlogDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public List<string> Images { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }  
    }
}

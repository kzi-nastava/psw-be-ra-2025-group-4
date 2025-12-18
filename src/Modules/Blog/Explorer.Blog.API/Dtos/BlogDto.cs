using System;
using System.Collections.Generic;

namespace Explorer.Blog.API.Dtos
{
    public enum BlogPopularityDTO
    {
        None = 0,
        Active = 1,
        Famous = 2
    }
    public enum BlogStatusDTO
    {
        Preparation = 0,
        Published = 1,
        Archived = 2
    }
    public class BlogDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public List<string> Images { get; set; }
        public int UserId { get; set; }
        public BlogStatusDTO Status { get; set; }
        public BlogPopularityDTO Popularity { get; set; }


    }
}

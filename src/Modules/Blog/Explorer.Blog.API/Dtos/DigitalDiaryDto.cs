using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class DigitalDiaryDto
    {
        public long Id { get; set; }
        public long TouristId { get; set; }          
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }     
        public string Status { get; set; } = "Draft"; 
        public string Country { get; set; } = string.Empty;
        public string? City { get; set; }
    }
}

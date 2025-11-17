using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string Status { get; set; }
        public decimal Price { get; set; }
        public int AuthorId { get; set; }
    }
}

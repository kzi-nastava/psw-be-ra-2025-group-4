using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class RatingDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public int Value { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}

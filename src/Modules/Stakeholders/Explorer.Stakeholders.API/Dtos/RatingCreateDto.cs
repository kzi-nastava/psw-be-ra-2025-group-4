using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class RatingCreateDto
    {
        public long UserId { get; set; }
        public int Value { get; set; }
        public string Comment { get; set; }
    }
}

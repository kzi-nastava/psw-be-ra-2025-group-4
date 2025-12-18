using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long OwnerId { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public string? Status { get; set; } 
        public List<long> Members { get; set; } = new();
        public List<long> InvitedTourist { get; set; } = new();
        public List<long> RequestedTourists { get; set; } = new();
    }
}

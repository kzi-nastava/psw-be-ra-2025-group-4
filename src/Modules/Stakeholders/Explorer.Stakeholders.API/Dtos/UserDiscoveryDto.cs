using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class UserDiscoveryDto
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}

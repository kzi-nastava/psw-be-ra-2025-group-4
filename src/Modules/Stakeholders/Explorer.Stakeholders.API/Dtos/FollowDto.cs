using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class FollowDto
    {
        public long FollowerId { get; set; }
        public long FollowedId { get; set; }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class UserDto
    {
        public string Username { get; private set; }
        public string Role { get; private set; }
        public bool IsActive { get; set; }
    }
}

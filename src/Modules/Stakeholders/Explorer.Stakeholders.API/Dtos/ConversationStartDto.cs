using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ConversationStartDto
    {
        public string Username { get; set; }
        public string Content { get; set; }
        public string? ResourceUrl { get; set; }
    }
}

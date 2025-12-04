using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class DirectMessageDto
    {
        public long Id { get; set; }
        public string Sender {  get; set; }
        public long SenderId { get; set; }
        public string Recipient { get; set; }
        public long RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public string? ResourceUrl { get; set; }

    }
}

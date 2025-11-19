using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Explorer.Stakeholders.Core.Domain
{
    public class DirectMessage : Entity
    {
        public Person Sender { get; set; }
        public long SenderId { get; set; }
        public Person Recipient { get; set; }
        public long RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? EditedAt { get; set; }

        public DirectMessage() { }

        public DirectMessage(long senderId, long recipientId, string content, DateTime sentAt)
        {
            SenderId = senderId;
            RecipientId = recipientId;
            Content = content;
            SentAt = sentAt;
            EditedAt = null;
            Validate();
        }

        private void Validate()
        {
            if (SenderId == 0 || RecipientId == 0) throw new ArgumentException("Invalid UserId");
            if (string.IsNullOrWhiteSpace(Content)) throw new ArgumentException("Invalid Message");
        }
    }
}

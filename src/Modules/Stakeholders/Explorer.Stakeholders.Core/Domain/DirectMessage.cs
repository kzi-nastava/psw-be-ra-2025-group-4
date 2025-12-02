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
        public User Sender { get; set; }
        public long SenderId { get; set; }
        public User Recipient { get; set; }
        public long RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public long? ResourceId { get; set; }
        public ResourceType ResourceType { get; set; } = ResourceType.None;


        public DirectMessage() { }

        public DirectMessage(long senderId, long recipientId, string content, DateTime sentAt, long? resourceId, ResourceType resourceType)
        {
            SenderId = senderId;
            RecipientId = recipientId;
            Content = content;
            SentAt = sentAt;
            EditedAt = null;
            ResourceId = resourceId;
            ResourceType = resourceType;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Content)) throw new ArgumentException("Invalid Message");
            
            if (Content.Length > 280)
                throw new ArgumentException("Message too long (max 280 characters)");
        }
    }
}
public enum ResourceType
    {
        None = 0,
        Tour = 1,
        Blog = 2
    }
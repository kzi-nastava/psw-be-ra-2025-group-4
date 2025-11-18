using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain
{
    public class DigitalDiary : Entity
    {
        public long TouristId { get; private set; }
        public string Title { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Status { get; private set; } 
        public string Country { get; private set; }
        public string? City { get; private set; }

        public DigitalDiary(long touristId, string title, DateTime createdAt, string status, string country, string? city)
        {
            if (touristId == 0)
                throw new ArgumentException("Invalid TouristId.");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Invalid Title.");

            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Invalid Status.");

            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Invalid Country.");

            if (createdAt > DateTime.UtcNow)
                throw new ArgumentException("Creation date cannot be in the future.");

            Title = title;
            CreatedAt = createdAt;
            Status = status;
            Country = country;
            City = city;
        }

        public void UpdateTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Invalid Title.");
            Title = newTitle;
        }

        public void UpdateLocation(string country, string? city)
        {
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Invalid Country.");
            Country = country;
            City = city;
        }

        public void ChangeStatus(string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                throw new ArgumentException("Invalid Status.");
            Status = newStatus;
        }
    }
}

using System;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public enum TourStatus
    {
        Draft,
        Published, 
        Archived
    }

    public enum TourDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public class Tour : Entity
    {
        public int Id { get; set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TourDifficulty Difficulty { get; private set; }
        public List<string> Tags { get; private set; } = new List<string>();
        public TourStatus Status { get; private set; } = TourStatus.Draft;
        public decimal Price { get; private set; } = 0;
        public int AuthorId { get; private set; }

        public Tour(string name, string description, TourDifficulty difficulty, int authorId, List<string>? tags = null)
        {
            Name = name;
            Description = description;
            Difficulty = difficulty;
            AuthorId = authorId;
            if (tags != null) Tags = tags;

            Validate();
        }
        public void SetStatus(TourStatus status)
        {
            Status = status;
        }

        public void SetPrice(decimal price)
        {
            Price = price;
        }

        public void Update(string name, string description, TourDifficulty difficulty, List<string> tags)
        {
            Name = name;
            Description = description;
            Difficulty = difficulty;
            Tags = tags ?? new List<string>();
            Validate();
        }
        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Name cannot be empty");
            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Description cannot be empty");

        }
    }
}

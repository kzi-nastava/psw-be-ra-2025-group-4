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

    public class Tour : AggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public TourDifficulty Difficulty { get; private set; }
        public List<string> Tags { get; private set; } = new List<string>();
        public TourStatus Status { get; private set; } = TourStatus.Draft;
        public int AuthorId { get; private set; }
        public List<TourPoint> Points { get; private set; } = new();
        public List<Equipment> Equipment { get; private set; } = new List<Equipment>();
        public Money Price { get; private set; }
        public List<TourTransportDuration> TransportDuration { get; private set; } = new List<TourTransportDuration>();
        public DateTime? PublishedAt { get; private set; }
        public DateTime? ArchivedAt { get; private set; }

        private Tour()
        {

        }

        public Tour(long id, string name, string description, TourDifficulty difficulty, List<string> tags, TourStatus status, int authorId, List<TourPoint> points, List<Equipment> equipment, Money price, List<TourTransportDuration> transportDuration, DateTime? publishedAt, DateTime? archivedAt)
        {
            Id = id;
            Name = name;
            Description = description;
            Difficulty = difficulty;
            Tags = tags;
            Status = status;
            AuthorId = authorId;
            Points = points;
            Equipment = equipment;
            Price = price;
            TransportDuration = transportDuration;
            PublishedAt = publishedAt;
            ArchivedAt = archivedAt;
        }

        public Tour(string name, string description, TourDifficulty difficulty, int authorId, List<TourTransportDuration> transportDurations, List<string>? tags = null            )
        {
            Name = name;
            Description = description;
            Difficulty = difficulty;
            AuthorId = authorId;
            Status = TourStatus.Draft;
            Price = new Money(0.00m, "RSD");
            TransportDuration = transportDurations;
            if (tags != null) Tags = tags;

            Validate();
        }
        public void SetStatus(TourStatus status)
        {
            Status = status;
        }

        public void Update(string name, string description, TourDifficulty difficulty, List<TourTransportDuration> transportDurations, List<string> tags)
        {
            Name = name;
            Description = description;
            Difficulty = difficulty;
            TransportDuration = transportDurations;
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

        public void Publish()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description)
                || Points.Count < 2 || TransportDuration.Count == 0 || Tags.Count == 0 || Status == TourStatus.Published)
            {
                throw new ArgumentException("Tour canot be published.");
            }

            Status = TourStatus.Published;
            PublishedAt = DateTime.UtcNow;
        }

        public void Archive()
        {
            if (Status != TourStatus.Published)
                throw new InvalidOperationException("Only published tours can be archived.");

            Status = TourStatus.Archived;
            ArchivedAt = DateTime.UtcNow;
        }

        public void SetPrice(Money price)
        {
            Price = price;
        }

        public void AddTourPoint(TourPoint point)
        {
            Points.Add(point);
        }

        public void AddEquipment(Equipment equipment)
        {

            Equipment.Add(equipment);
        }

        public void AddEquipments(List<Equipment> equipments)
        {

            Equipment.AddRange(equipments);
        }

        public void AddTransportDuration(TourTransportDuration duration)
        {
            TransportDuration.Add(duration);
        }
    }
}

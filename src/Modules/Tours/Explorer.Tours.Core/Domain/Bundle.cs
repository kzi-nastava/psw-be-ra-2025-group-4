using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.Domain
{
    public enum BundleStatus
    {
        Draft,
        Published,
        Archived
    }

    public class Bundle : AggregateRoot
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public int AuthorId { get; private set; }
        public BundleStatus Status { get; private set; } = BundleStatus.Draft;
        public List<Tour> Tours { get; private set; } = new List<Tour>();

        private Bundle()
        {
        }

        public Bundle(string name, decimal price, int authorId, List<Tour> tours)
        {
            Name = name;
            Price = price;
            AuthorId = authorId;
            Tours = tours ?? new List<Tour>();
            Status = BundleStatus.Draft;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Name cannot be empty");
            if (Price < 0)
                throw new ArgumentException("Price cannot be negative");
            if (Tours == null || Tours.Count == 0)
                throw new ArgumentException("Bundle must contain at least one tour");
        }

        public void Update(string name, decimal price, List<Tour> tours)
        {
            if (Status == BundleStatus.Archived)
                throw new InvalidOperationException("Cannot modify an archived bundle.");

            Name = name;
            Price = price;
            Tours = tours ?? new List<Tour>();
            Validate();
        }

        public void Publish()
        {
            if (Status == BundleStatus.Published)
                throw new InvalidOperationException("Bundle is already published.");

            var publishedToursCount = Tours.Count(t => t.Status == TourStatus.Published);
            if (publishedToursCount < 2)
                throw new InvalidOperationException("Bundle must contain at least two published tours to be published.");

            Status = BundleStatus.Published;
        }

        public void Archive()
        {
            if (Status != BundleStatus.Published)
                throw new InvalidOperationException("Only published bundles can be archived.");

            Status = BundleStatus.Archived;
        }

        public void Delete()
        {
            if (Status == BundleStatus.Published)
                throw new InvalidOperationException("Published bundles cannot be deleted. Archive them instead.");
        }
    }
}


using System;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourReview : Entity
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public List<string> Images { get; set; }
        public DateTime CreatedAt { get; set; }
        public double TourCompletionPercentage { get; set; }

        public TourReview(int tourId, int touristId, int rating, string comment,
            List<string> images, DateTime createdAt, double tourCompletionPercentage)
        {
            TourId = tourId;
            TouristId = touristId;
            Rating = rating;
            Comment = comment;
            Images = images ?? new List<string>();
            CreatedAt = createdAt;
            TourCompletionPercentage = tourCompletionPercentage;
            Validate();
        }

        private void Validate()
        {
            if (Rating < 1 || Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            if (string.IsNullOrWhiteSpace(Comment))
                throw new ArgumentException("Comment cannot be empty");

            if (TourId == 0)
                throw new ArgumentException("TourId must not be zero");

            if (TouristId == 0)
                throw new ArgumentException("TouristId must not be zero");

            if (TourCompletionPercentage < 0 || TourCompletionPercentage > 100)
                throw new ArgumentException("Tour completion percentage must be between 0 and 100");
        }
    }
}
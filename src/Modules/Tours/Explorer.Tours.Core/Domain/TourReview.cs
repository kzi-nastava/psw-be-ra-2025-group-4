using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class TourReview : Entity
    {
        public long TouristId { get; private set; }
        public int TourId { get; private set; }
        public int Rating { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public double CompletionPercentage { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        private TourReview() { }

        public TourReview(long touristId, int tourId, int rating, string comment, double completionPercentage)
        {
            ValidateTouristId(touristId);
            ValidateTourId(tourId);
            ValidateRating(rating);
            ValidateComment(comment);
            ValidateCompletionPercentage(completionPercentage);

            TouristId = touristId;
            TourId = tourId;
            Rating = rating;
            Comment = comment;
            CompletionPercentage = completionPercentage;
            CreatedAt = DateTime.UtcNow;
            LastModifiedAt = null;
        }

        public void Update(int rating, string comment)
        {
            ValidateRating(rating);
            ValidateComment(comment);

            Rating = rating;
            Comment = comment;
            LastModifiedAt = DateTime.UtcNow;
        }

        private void ValidateTouristId(long touristId)
        {
            if (touristId == 0)
            {
                throw new ArgumentException("Tourist ID mora biti validan.", nameof(touristId));
            }
        }

        private void ValidateTourId(int tourId)
        {
            if (tourId == 0)
            {
                throw new ArgumentException("Tour ID mora biti validan.", nameof(tourId));
            }
        }

        private void ValidateRating(int rating)
        {
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentException("Ocena mora biti između 1 i 5.");
            }
        }

        private void ValidateComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentException("Komentar je obavezan!");
            }

            if (comment.Length < 10)
            {
                throw new ArgumentException("Komentar mora sadržati minimum 10 karaktera!");
            }

            if (comment.Length > 1000)
            {
                throw new ArgumentException("Komentar može sadržati maksimum 1000 karaktera!");
            }
        }

        private void ValidateCompletionPercentage(double percentage)
        {
            if (percentage < 0 || percentage > 100)
            {
                throw new ArgumentException("Procenat mora biti između 0 i 100.");
            }
        }
    }
}


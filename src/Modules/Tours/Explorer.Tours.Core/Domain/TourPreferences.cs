using System;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourPreferences : Entity
    {
        public int TouristId { get; private set; }

        public TourDifficulty PreferredDifficulty { get; private set; }

        public int WalkRating { get; private set; }
        public int BikeRating { get; private set; }
        public int CarRating { get; private set; }
        public int BoatRating { get; private set; }

        public List<string> Tags { get; private set; } = new();

        
        private TourPreferences() { }

        public TourPreferences(
            int touristId,
            TourDifficulty preferredDifficulty,
            int walkRating,
            int bikeRating,
            int carRating,
            int boatRating,
            List<string>? tags = null)
        {
            TouristId = touristId;
            PreferredDifficulty = preferredDifficulty;
            WalkRating = walkRating;
            BikeRating = bikeRating;
            CarRating = carRating;
            BoatRating = boatRating;

            if (tags != null) Tags = tags;

            Validate();
        }

        public void Update(
            TourDifficulty preferredDifficulty,
            int walkRating,
            int bikeRating,
            int carRating,
            int boatRating,
            List<string>? tags)
        {
            PreferredDifficulty = preferredDifficulty;
            WalkRating = walkRating;
            BikeRating = bikeRating;
            CarRating = carRating;
            BoatRating = boatRating;
            Tags = tags ?? new List<string>();

            Validate();
        }

        private void Validate()
        {
            //if (TouristId == 0)
                //throw new ArgumentException("TouristId must be greater than 0.");

            ValidateRating(WalkRating, nameof(WalkRating));
            ValidateRating(BikeRating, nameof(BikeRating));
            ValidateRating(CarRating, nameof(CarRating));
            ValidateRating(BoatRating, nameof(BoatRating));
        }

        private static void ValidateRating(int rating, string fieldName)
        {
            if (rating < 0 || rating > 3)
                throw new ArgumentException($"{fieldName} must be between 0 and 3.");
        }
    }
}

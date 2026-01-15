using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class HiddenLocationEncounter : Encounter
    {
        public string ImageUrl { get; private set; }
        public Location PhotoPoint { get; private set; }
        public double ActivationRadiusMeters { get; private set; }

        public double CompletionRadiusMeters => 500.0;
        public int CompletionHoldSeconds => 30;

        protected HiddenLocationEncounter() { }

        public HiddenLocationEncounter(
            string name,
            string description,
            Location location,
            int experiencePoints,
            string imageUrl,
            Location photoPoint,
            double activationRadiusMeters
        ) : base(
            name,
            description,
            location,
            experiencePoints,
            EncounterType.Location
        )
        {
            ImageUrl = imageUrl;
            PhotoPoint = photoPoint;
            ActivationRadiusMeters = activationRadiusMeters;
        }

        public void UpdateHiddenData(
            string imageUrl,
            Location photoPoint,
            double activationRadiusMeters
        )
        {
            ImageUrl = imageUrl;
            PhotoPoint = photoPoint;
            ActivationRadiusMeters = activationRadiusMeters;
        }
    }
}

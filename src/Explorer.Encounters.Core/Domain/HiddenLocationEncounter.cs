using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class HiddenLocationEncounter : Encounter
    {
        public string ImageUrl { get; private set; }
        public Location PhotoPoint { get; private set; }
        public double ActivationRadiusMeters { get; private set; }

        public double CompletionRadiusMeters => 500.0;
        public int CompletionHoldSeconds => 10;

        protected HiddenLocationEncounter() { }

        public HiddenLocationEncounter(
            string name,
            string description,
            Location location,
            int experiencePoints,
            string imageUrl,
            Location photoPoint,
            double activationRadiusMeters,
            EncounterApprovalStatus approvalStatus
        ) : base(
            name,
            description,
            location,
            experiencePoints,
            EncounterType.Location,
            approvalStatus
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

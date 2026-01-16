using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.API.Dtos
{
    public class HiddenLocationEncounterDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public LocationDto Location { get; set; } = null!;

        public int ExperiencePoints { get; set; }
        public long? TourPointId { get; set; }
        public bool IsRequiredForPointCompletion { get; set; }

        public string ImageUrl { get; set; } = string.Empty;
        public LocationDto PhotoPoint { get; set; } = null!;
        public double ActivationRadiusMeters { get; set; }
    }
}

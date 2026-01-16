
namespace Explorer.Encounters.API.Dtos
{
    public class EncounterViewDto
    {
        // base
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public LocationDto Location { get; set; }
        public int ExperiencePoints { get; set; }
        public EncounterType Type { get; set; }
        public long? TourPointId { get; set; }
        public bool? IsRequiredForPointCompletion { get; set; }
        public bool CanActivate { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        // ==== HiddenLocation ONLY ====
        public string? ImageUrl { get; set; }
        public LocationDto? PhotoPoint { get; set; }
        public double? ActivationRadiusMeters { get; set; }
        public int? MinimumParticipants { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
    public enum EncounterStatus
    {
        Active,
        Draft,
        Archived
    }

    public enum EncounterType
    {
        Social,
        Location,
        Misc
    }

    public class EncounterDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public LocationDto Location { get; set; }
        public int ExperiencePoints { get; set; }
        public EncounterStatus Status { get; set; }
        public EncounterType Type { get; set; }
        public long? TourPointId { get; set; }
        public bool? IsRequiredForPointCompletion { get; set; }
        public List<long> TouristsStarted { get; set; } = new();
        public List<long> TouristsCompleted { get; set; } = new();
    }
}

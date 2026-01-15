using System;

namespace Explorer.Encounters.API.Dtos
{
    public enum EncounterExecutionStatusDto
    {
        Started = 0,
        Completed = 1
    }

    public class EncounterExecutionDto
    {
        public long Id { get; set; }
        public long EncounterId { get; set; }
        public long TouristId { get; set; }
        public EncounterExecutionStatusDto Status { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public DateTime? WithinRadiusSinceUtc { get; set; }
    }
}

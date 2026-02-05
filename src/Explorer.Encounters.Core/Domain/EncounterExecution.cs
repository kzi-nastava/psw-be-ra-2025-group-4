using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public enum EncounterExecutionStatus
    {
        Started,
        Completed
    }

    public class EncounterExecution : AggregateRoot
    {
        public long EncounterId { get; private set; }
        public long TouristId { get; private set; }

        public EncounterExecutionStatus Status { get; private set; }

        public DateTime StartedAtUtc { get; private set; }
        public DateTime? CompletedAtUtc { get; private set; }

        // Samo za Hidden i social
        public DateTime? WithinRadiusSinceUtc { get; private set; }

        private EncounterExecution() { }

        public EncounterExecution(long touristId, long encounterId)
        {
            TouristId = touristId;
            EncounterId = encounterId;
            Status = EncounterExecutionStatus.Started;
            StartedAtUtc = DateTime.UtcNow;
        }

        public void SetWithinRadius(DateTime sinceUtc)
        {
            WithinRadiusSinceUtc = sinceUtc;
        }

        public void EnterRadius()
        {
            if (WithinRadiusSinceUtc == null)
                WithinRadiusSinceUtc = DateTime.UtcNow;
        }

        public void LeaveRadius()
        {
            WithinRadiusSinceUtc = null;
        }

        public void Complete()
        {
            Status = EncounterExecutionStatus.Completed;
            CompletedAtUtc = DateTime.UtcNow;
        }

        public void SetStartedAt(DateTime startedAtUtc)
        {
            StartedAtUtc = startedAtUtc;
        }
    }
}

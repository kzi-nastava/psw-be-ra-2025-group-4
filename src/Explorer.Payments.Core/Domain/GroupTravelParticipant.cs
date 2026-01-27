using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public enum GroupTravelParticipantStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }

    public class GroupTravelParticipant : Entity
    {
        public int TouristId { get; private set; }
        public GroupTravelParticipantStatus Status { get; private set; }
        public DateTime? RespondedAt { get; private set; }

        private GroupTravelParticipant() { }

        public GroupTravelParticipant(int touristId)
        {
            if (touristId == 0) throw new System.ArgumentException("Invalid tourist id.", nameof(touristId));
            TouristId = touristId;
            Status = GroupTravelParticipantStatus.Pending;
        }

        public void Accept()
        {
            if (Status != GroupTravelParticipantStatus.Pending)
                throw new System.InvalidOperationException("Participant has already responded.");
            
            Status = GroupTravelParticipantStatus.Accepted;
            RespondedAt = System.DateTime.UtcNow;
        }

        public void Reject()
        {
            if (Status != GroupTravelParticipantStatus.Pending)
                throw new System.InvalidOperationException("Participant has already responded.");
            
            Status = GroupTravelParticipantStatus.Rejected;
            RespondedAt = System.DateTime.UtcNow;
        }
    }
}

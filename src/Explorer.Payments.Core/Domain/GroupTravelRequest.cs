using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public enum GroupTravelStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
        Completed = 3,
        Cancelled = 4
    }

    public class GroupTravelRequest : AggregateRoot
    {
        public int OrganizerId { get; private set; }
        public int TourId { get; private set; }
        public string TourName { get; private set; }
        public decimal PricePerPerson { get; private set; }
        public GroupTravelStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public List<GroupTravelParticipant> Participants { get; private set; }

        private GroupTravelRequest() 
        {
            Participants = new List<GroupTravelParticipant>();
        }

        public GroupTravelRequest(int organizerId, int tourId, string tourName, decimal pricePerPerson, List<int> participantIds)
        {
            if (organizerId == 0) throw new ArgumentException("Invalid organizer id.", nameof(organizerId));
            if (tourId == 0) throw new ArgumentException("Invalid tour id.", nameof(tourId));
            if (string.IsNullOrWhiteSpace(tourName)) throw new ArgumentException("Tour name cannot be empty.", nameof(tourName));
            if (pricePerPerson <= 0) throw new ArgumentException("Price must be positive.", nameof(pricePerPerson));
            if (participantIds == null || participantIds.Count == 0) throw new ArgumentException("At least one participant is required.", nameof(participantIds));
            if (participantIds.Contains(organizerId)) throw new ArgumentException("Organizer cannot be a participant.", nameof(participantIds));

            OrganizerId = organizerId;
            TourId = tourId;
            TourName = tourName;
            PricePerPerson = pricePerPerson;
            Status = GroupTravelStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            Participants = participantIds.Select(id => new GroupTravelParticipant(id)).ToList();
        }

        public void AcceptParticipant(int participantId)
        {
            var participant = Participants.FirstOrDefault(p => p.TouristId == participantId);
            if (participant == null) throw new ArgumentException("Participant not found.", nameof(participantId));
            if (participant.Status != GroupTravelParticipantStatus.Pending) 
                throw new InvalidOperationException("Participant has already responded.");

            participant.Accept();
            UpdateStatus();
        }

        public void RejectParticipant(int participantId)
        {
            var participant = Participants.FirstOrDefault(p => p.TouristId == participantId);
            if (participant == null) throw new ArgumentException("Participant not found.", nameof(participantId));
            if (participant.Status != GroupTravelParticipantStatus.Pending) 
                throw new InvalidOperationException("Participant has already responded.");

            participant.Reject();
            UpdateStatus();
        }

        public void Complete()
        {
            if (Status != GroupTravelStatus.Accepted)
                throw new InvalidOperationException("Cannot complete request that is not accepted by all participants.");

            Status = GroupTravelStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == GroupTravelStatus.Completed)
                throw new InvalidOperationException("Cannot cancel completed request.");

            Status = GroupTravelStatus.Cancelled;
        }

        private void UpdateStatus()
        {
            if (Status == GroupTravelStatus.Cancelled) return;

            var allAccepted = Participants.All(p => p.Status == GroupTravelParticipantStatus.Accepted);
            var anyRejected = Participants.Any(p => p.Status == GroupTravelParticipantStatus.Rejected);

            if (allAccepted)
            {
                Status = GroupTravelStatus.Accepted;
            }
            else if (anyRejected)
            {
                Status = GroupTravelStatus.Rejected;
            }
        }

        public bool AllParticipantsAccepted()
        {
            return Participants.All(p => p.Status == GroupTravelParticipantStatus.Accepted);
        }

        public List<int> GetAcceptedParticipantIds()
        {
            return Participants
                .Where(p => p.Status == GroupTravelParticipantStatus.Accepted)
                .Select(p => p.TouristId)
                .ToList();
        }
    }
}

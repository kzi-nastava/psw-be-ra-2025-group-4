using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
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

    public enum EncounterApprovalStatus
    {
        APPROVED,
        PENDING,
        DECLINED
    }

    public class Encounter : AggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Location Location { get; private set; }
        public int ExperiencePoints { get; private set; }
        public EncounterStatus Status { get; private set; }
        public EncounterType Type { get; private set; }

        public long? TourPointId { get; private set; }
        public bool IsRequiredForPointCompletion { get; private set; }

        public EncounterApprovalStatus ApprovalStatus { get; private set; }


        public Encounter()
        {

        }

        public Encounter(string name, string description, Location location, int experiencePoints, EncounterType type, EncounterApprovalStatus approvalStatus)
        {
            Name = name;
            Description = description;
            Location = location;
            ExperiencePoints = experiencePoints;
            Status = EncounterStatus.Draft;
            Type = type;
            Validate();
            ApprovalStatus = approvalStatus;
        }

        public void Update(string name, string description, Location location, int experiencePoints, EncounterType type)
        {
            Name = name;
            Description = description;
            Location = location;
            ExperiencePoints = experiencePoints;
            Type = type;

            Validate();
        }

        public void Approve()
        {
            if (ApprovalStatus == EncounterApprovalStatus.APPROVED)
                throw new InvalidOperationException("Encounter already approved.");

            ApprovalStatus = EncounterApprovalStatus.APPROVED;
        }

        public void Decline()
        {
            if (ApprovalStatus == EncounterApprovalStatus.DECLINED)
                throw new InvalidOperationException("Encounter already declined.");

            ApprovalStatus = EncounterApprovalStatus.DECLINED;
        }


        public void Activate()
        {
            //TODO: U drugoj nedelji ovde dodati ostale uslove (po potrebi)
            if (Status == EncounterStatus.Active)
                throw new InvalidOperationException("Encounter is already active.");
            if (ApprovalStatus != EncounterApprovalStatus.APPROVED)
                throw new InvalidOperationException("Only approved encounters can be activated.");

            Status = EncounterStatus.Active;
        }

        public void Archive()
        {
            //TODO: U drugoj nedelji ovde dodati ostale uslove (po potrebi)
            if (Status == EncounterStatus.Archived)
                throw new InvalidOperationException("Encounter is already archived.");
            
            Status = EncounterStatus.Archived;
        }

        public void SetTourPoint(long tourPointId, bool isRequiredForPointCompletion)
        {
            TourPointId = tourPointId;
            IsRequiredForPointCompletion = isRequiredForPointCompletion;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Encounter name cannot be empty.");

            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Encounter description cannot be empty.");

            if (ExperiencePoints < 0)
                throw new ArgumentOutOfRangeException(nameof(ExperiencePoints), "Experience points cannot be negative.");
        }
    }
}

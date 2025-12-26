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

    public class Encounter : AggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Location Location { get; private set; }
        public int ExperiencePoints { get; private set; }
        public EncounterStatus Status { get; private set; }
        public EncounterType Type { get; private set; }
        private Encounter()
        {

        }

        public Encounter(string name, string description, Location location, int experiencePoints, EncounterType type)
        {
            Name = name;
            Description = description;
            Location = location;
            ExperiencePoints = experiencePoints;
            Status = EncounterStatus.Draft;
            Type = type;

            Validate();
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

        public void Activate()
        {
            //TODO: U drugoj nedelji ovde dodati ostale uslove (po potrebi)
            if (Status == EncounterStatus.Active)
                throw new InvalidOperationException("Encounter is already active.");

            Status = EncounterStatus.Active;
        }

        public void Archive()
        {
            //TODO: U drugoj nedelji ovde dodati ostale uslove (po potrebi)
            if (Status == EncounterStatus.Archived)
                throw new InvalidOperationException("Encounter is already archived.");
            
            Status = EncounterStatus.Archived;
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

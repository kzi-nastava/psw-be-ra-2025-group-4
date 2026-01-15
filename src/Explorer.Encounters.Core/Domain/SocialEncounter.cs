using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class SocialEncounter : Encounter
    {
        public int MinimumParticipants { get; private set; }
        public double ActivationRadiusMeters { get; private set; }

        public ICollection<SocialEncounterParticipant> Participants { get; private set; } = new List<SocialEncounterParticipant>();

        private SocialEncounter()
        {

        }

        public SocialEncounter(string name, string description, Location location, int experiencePoints, int minimumParticipants, double radius, EncounterApprovalStatus approvalStatus)
            : base(name, description, location, experiencePoints, EncounterType.Social, approvalStatus)
        {
            MinimumParticipants = minimumParticipants;
            ActivationRadiusMeters = radius;
        }

        public void UpdateSocial(
            string name,
            string description,
            Location location,
            int experiencePoints,
            int minimumParticipants,
            double activationRadiusMeters)
        {
            base.Update(name, description, location, experiencePoints, EncounterType.Social);

            MinimumParticipants = minimumParticipants;
            ActivationRadiusMeters = activationRadiusMeters;
        }

    }
}

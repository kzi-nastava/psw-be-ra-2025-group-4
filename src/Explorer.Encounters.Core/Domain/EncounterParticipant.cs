using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Encounters.Core.Domain
{
    public class EncounterParticipant : Entity
    {
        public long UserId { get; private set; }

        public int Level { get; private set; }

        public int ExperiencePoints { get; private set; }

        public DateTime? LastCompletedEncounterAtUtc { get; private set; }
        public bool CanCreateEncounter => Level >= 10;

        protected EncounterParticipant() { }

        public EncounterParticipant(long userId)
        {
            if (userId <= 0)
                throw new ArgumentException("UserId must be positive.", nameof(userId));

            UserId = userId;
            Level = 1;
            ExperiencePoints = 0;
        }

   
        public void AddExperience(int xp)
        {
            if (xp <= 0)
                throw new ArgumentOutOfRangeException(nameof(xp), "XP must be positive.");

            ExperiencePoints += xp;

            while (ExperiencePoints >= ExperiencePointsForNextLevel())
            {
                ExperiencePoints -= ExperiencePointsForNextLevel();
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
        }

        private int ExperiencePointsForNextLevel()
        {
            return (int)(36 * Math.Pow(1.5, Level - 1));
        }

        public void CompleteEncounter()
        {
            LastCompletedEncounterAtUtc = DateTime.UtcNow;
        }
    }
}

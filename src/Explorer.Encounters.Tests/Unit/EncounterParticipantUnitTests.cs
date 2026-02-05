using Explorer.Encounters.Core.Domain;
using System;
using Xunit;

namespace Explorer.Encounters.Tests.Unit
{
    [Collection("Sequential")]
    public class EncounterParticipantUnitTests
    {
        [Fact]
        public void Constructor_Should_Set_UserId_And_Defaults()
        {
            long userId = 42;

            var participant = new EncounterParticipant(userId);

            Assert.Equal(userId, participant.UserId);
            Assert.Equal(1, participant.Level);
            Assert.Equal(0, participant.ExperiencePoints);
            Assert.Null(participant.LastCompletedEncounterAtUtc);
            Assert.False(participant.CanCreateEncounter);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(14)]
        [InlineData(10)]
        public void AddExperience_Should_Increase_ExperiencePoints(int xp)
        {
            var participant = new EncounterParticipant(1);

            participant.AddExperience(xp);

            Assert.Equal(xp, participant.ExperiencePoints);
            Assert.Equal(1, participant.Level);
        }

        [Fact]
        public void AddExperience_Should_LevelUp_When_Threshold_Exceeded()
        {
            var participant = new EncounterParticipant(1);

            int xpToNextLevel = 36; 
            participant.AddExperience(xpToNextLevel);

            Assert.Equal(2, participant.Level);
            Assert.Equal(0, participant.ExperiencePoints);
        }

        [Fact]
        public void AddExperience_Should_LevelUp_Multiple_Times()
        {
            var participant = new EncounterParticipant(1);

            int xpForLevel1 = 36;
            int xpForLevel2 = (int)(36 * Math.Pow(1.5, 2 - 1)); 
            int totalXp = xpForLevel1 + xpForLevel2;

            participant.AddExperience(totalXp);

            Assert.Equal(3, participant.Level);
            Assert.Equal(0, participant.ExperiencePoints);
        }

        [Fact]
        public void AddExperience_Should_Throw_When_Xp_Is_NonPositive()
        {
            var participant = new EncounterParticipant(1);

            Assert.Throws<ArgumentOutOfRangeException>(() => participant.AddExperience(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => participant.AddExperience(-10));
        }

        [Fact]
        public void CompleteEncounter_Should_Set_LastCompletedEncounterAtUtc()
        {
            var participant = new EncounterParticipant(1);

            participant.CompleteEncounter();

            Assert.NotNull(participant.LastCompletedEncounterAtUtc);
            Assert.True(participant.LastCompletedEncounterAtUtc <= DateTime.UtcNow);
        }

        [Fact]
        public void CanCreateEncounter_Should_Be_True_When_Level_At_Least_10()
        {
            var participant = new EncounterParticipant(1);

            for (int i = 1; i < 10; i++)
            {
                participant.AddExperience(int.MaxValue); 
            }

            Assert.True(participant.Level >= 10);
            Assert.True(participant.CanCreateEncounter);
        }

        [Fact]
        public void CanCreateEncounter_Should_Be_False_When_Level_Below_10()
        {
            var participant = new EncounterParticipant(1);

            Assert.True(participant.Level < 10);
            Assert.False(participant.CanCreateEncounter);
        }
    }
}

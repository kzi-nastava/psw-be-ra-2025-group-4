using Explorer.Encounters.Core.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests.Unit
{
    [Collection("Sequential")]
    public class EncounterAdminUnitTests
    {
        [Fact]
        public void Creates_encounter_with_valid_data()
        {
            var encounter = new Encounter(
                "Test Encounter",
                "Test description",
                new Location(19.8335, 45.2671), // longitude, latitude
                100,
                EncounterType.Social,
                EncounterApprovalStatus.APPROVED
            );

            encounter.Name.ShouldBe("Test Encounter");
            encounter.Description.ShouldBe("Test description");
            encounter.ExperiencePoints.ShouldBe(100);
            encounter.Status.ShouldBe(EncounterStatus.Draft);
            encounter.Type.ShouldBe(EncounterType.Social);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Creating_encounter_fails_for_empty_name(string name)
        {
            Should.Throw<ArgumentException>(() =>
                new Encounter(name, "desc", new Location(19.8335, 45.2671), 10, EncounterType.Misc, EncounterApprovalStatus.APPROVED)
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Creating_encounter_fails_for_empty_description(string description)
        {
            Should.Throw<ArgumentException>(() =>
                new Encounter("Name", description, new Location(19.8335, 45.2671), 10, EncounterType.Location, EncounterApprovalStatus.APPROVED)
            );
        }

        [Fact]
        public void Creating_encounter_fails_for_negative_experience()
        {
            Should.Throw<ArgumentOutOfRangeException>(() =>
                new Encounter("Name", "Desc", new Location(19.8335, 45.2671), -5, EncounterType.Location, EncounterApprovalStatus.APPROVED)
            );
        }

        [Fact]
        public void Updates_encounter_successfully()
        {
            var encounter = new Encounter(
                "Name",
                "Desc",
                new Location(19.8335, 45.2671),
                100,
                EncounterType.Social,
                EncounterApprovalStatus.APPROVED
            );

            encounter.Update(
                "Updated name",
                "Updated description",
                new Location(19.2, 45.1),
                200,
                EncounterType.Misc
            );

            encounter.Name.ShouldBe("Updated name");
            encounter.Description.ShouldBe("Updated description");
            encounter.ExperiencePoints.ShouldBe(200);
            encounter.Type.ShouldBe(EncounterType.Misc);
        }

        [Fact]
        public void Activates_encounter()
        {
            var encounter = new Encounter(
                "Name",
                "Desc",
                new Location(19.8335, 45.2671),
                100,
                EncounterType.Social,
                EncounterApprovalStatus.APPROVED
            );

            encounter.Activate();

            encounter.Status.ShouldBe(EncounterStatus.Active);
        }

        [Fact]
        public void Activating_active_encounter_throws()
        {
            var encounter = new Encounter(
                "Name",
                "Desc",
                new Location(19.8335, 45.2671),
                100,
                EncounterType.Social,
                EncounterApprovalStatus.APPROVED
            );

            encounter.Activate();

            Should.Throw<InvalidOperationException>(() => encounter.Activate());
        }

        [Fact]
        public void Archives_encounter()
        {
            var encounter = new Encounter(
                "Name",
                "Desc",
                new Location(19.8335, 45.2671),
                100,
                EncounterType.Social,
                EncounterApprovalStatus.APPROVED
            );

            encounter.Archive();

            encounter.Status.ShouldBe(EncounterStatus.Archived);
        }

        [Fact]
        public void Archiving_archived_encounter_throws()
        {
            var encounter = new Encounter(
                "Name",
                "Desc",
                new Location(19.8335, 45.2671),
                100,
                EncounterType.Social,
                EncounterApprovalStatus.APPROVED
            );

            encounter.Archive();

            Should.Throw<InvalidOperationException>(() => encounter.Archive());
        }

        [Fact]
        public void Approves_encounter_successfully()
        {
            var encounter = new Encounter(
                "Name",
                "Desc",
                new Location(19.8335, 45.2671),
                100,
                EncounterType.Social,
                EncounterApprovalStatus.PENDING
            );

            encounter.Approve();

            encounter.ApprovalStatus.ShouldBe(EncounterApprovalStatus.APPROVED);
        }

        [Fact]
        public void Approving_already_approved_encounter_throws()
        {
            var encounter = new Encounter(
                "Name",
                "Desc",
                new Location(19.8335, 45.2671),
                100,
                EncounterType.Social,
                EncounterApprovalStatus.APPROVED
            );

            Should.Throw<InvalidOperationException>(() => encounter.Approve());
        }

        [Fact]
        public void Declines_encounter_successfully()
        {
            var encounter = new Encounter(
                "Name",
                "Desc",
                new Location(19.8335, 45.2671),
                100,
                EncounterType.Social,
                EncounterApprovalStatus.PENDING
            );

            encounter.Decline();

            encounter.ApprovalStatus.ShouldBe(EncounterApprovalStatus.DECLINED);
        }

        [Fact]
        public void Declining_already_declined_encounter_throws()
        {
            var encounter = new Encounter(
                "Name",
                "Desc",
                new Location(19.8335, 45.2671),
                100,
                EncounterType.Social,
                EncounterApprovalStatus.DECLINED
            );

            Should.Throw<InvalidOperationException>(() => encounter.Decline());
        }

    }
}

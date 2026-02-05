using System;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Clubs
{
    public class ClubDomainTests
    {
        private const long OwnerId = 1;
        private const long TouristA = 2;
        private const long TouristB = 3;

        private static Club CreateClub(long ownerId = OwnerId)
        {
            return new Club(
                name: "Hikers",
                description: "Club for hiking lovers",
                ownerId: ownerId,
                imageUrls: new List<string> { "https://img/1.png" }
            );
        }

        [Fact]
        public void Close_succeeds_for_owner()
        {
            var club = CreateClub();

            club.Close(OwnerId);

            club.Status.ShouldBe(ClubStatus.Closed);
        }

        [Fact]
        public void Close_fails_for_non_owner()
        {
            var club = CreateClub();

            Should.Throw<ForbiddenException>(() => club.Close(TouristA));
        }

        [Fact]
        public void Activate_succeeds_for_owner()
        {
            var club = CreateClub();
            club.Close(OwnerId);

            club.Activate(OwnerId);

            club.Status.ShouldBe(ClubStatus.Active);
        }

        [Fact]
        public void Activate_fails_for_non_owner()
        {
            var club = CreateClub();

            Should.Throw<ForbiddenException>(() => club.Activate(TouristA));
        }

        [Fact]
        public void AddMember_adds_when_not_present()
        {
            var club = CreateClub();

            club.AddMember(TouristA);

            club.Members.ShouldContain(TouristA);
        }

        [Fact]
        public void AddMember_does_not_duplicate()
        {
            var club = CreateClub();
            club.AddMember(TouristA);

            club.AddMember(TouristA);

            club.Members.Count(m => m == TouristA).ShouldBe(1);
        }

        [Fact]
        public void RemoveMember_succeeds_for_owner_when_member_exists()
        {
            var club = CreateClub();
            club.AddMember(TouristA);

            club.RemoveMember(OwnerId, TouristA);

            club.Members.ShouldNotContain(TouristA);
        }

        [Fact]
        public void RemoveMember_fails_for_non_owner()
        {
            var club = CreateClub();
            club.AddMember(TouristA);

            Should.Throw<ForbiddenException>(() => club.RemoveMember(TouristA, TouristA));
        }

        [Fact]
        public void RemoveMember_fails_when_tourist_is_not_member()
        {
            var club = CreateClub();

            Should.Throw<Exception>(() => club.RemoveMember(OwnerId, TouristA))
                .Message.ShouldBe("Tourist is not a member.");
        }

        [Fact]
        public void InviteMember_succeeds_when_active_and_owner()
        {
            var club = CreateClub();

            club.InviteMember(OwnerId, TouristA);

            club.InvitedTourist.ShouldContain(TouristA);
        }

        [Fact]
        public void InviteMember_fails_when_non_owner_invites()
        {
            var club = CreateClub();

            Should.Throw<ForbiddenException>(() => club.InviteMember(TouristA, TouristB));
        }

        [Fact]
        public void InviteMember_fails_when_club_is_closed()
        {
            var club = CreateClub();
            club.Close(OwnerId);

            Should.Throw<Exception>(() => club.InviteMember(OwnerId, TouristA))
                .Message.ShouldBe("Cannot invite members to a closed club.");
        }

        [Fact]
        public void InviteMember_fails_when_inviting_owner()
        {
            var club = CreateClub();

            Should.Throw<Exception>(() => club.InviteMember(OwnerId, OwnerId))
                .Message.ShouldBe("Owner cannot be invited.");
        }

        [Fact]
        public void InviteMember_fails_when_tourist_is_already_member()
        {
            var club = CreateClub();
            club.AddMember(TouristA);

            Should.Throw<Exception>(() => club.InviteMember(OwnerId, TouristA))
                .Message.ShouldBe("Tourist is already a member.");
        }

        [Fact]
        public void InviteMember_fails_when_invite_already_sent()
        {
            var club = CreateClub();
            club.InviteMember(OwnerId, TouristA);

            Should.Throw<Exception>(() => club.InviteMember(OwnerId, TouristA))
                .Message.ShouldBe("Invite already sent.");
        }

        [Fact]
        public void AcceptInvite_succeeds_moves_from_invited_to_members()
        {
            var club = CreateClub();
            club.InviteMember(OwnerId, TouristA);

            club.AcceptInvite(TouristA);

            club.InvitedTourist.ShouldNotContain(TouristA);
            club.Members.ShouldContain(TouristA);
        }

        [Fact]
        public void AcceptInvite_fails_when_club_is_closed()
        {
            var club = CreateClub();
            club.InviteMember(OwnerId, TouristA);
            club.Close(OwnerId);

            Should.Throw<Exception>(() => club.AcceptInvite(TouristA))
                .Message.ShouldBe("Cannot accept invite for a closed club.");
        }

        [Fact]
        public void AcceptInvite_fails_when_invite_does_not_exist()
        {
            var club = CreateClub();

            Should.Throw<Exception>(() => club.AcceptInvite(TouristA))
                .Message.ShouldBe("Invite does not exist.");
        }

        [Fact]
        public void DeclineInvite_succeeds_removes_invite()
        {
            var club = CreateClub();
            club.InviteMember(OwnerId, TouristA);

            club.DeclineInvite(TouristA);

            club.InvitedTourist.ShouldNotContain(TouristA);
            club.Members.ShouldNotContain(TouristA);
        }

        [Fact]
        public void DeclineInvite_fails_when_invite_does_not_exist()
        {
            var club = CreateClub();

            Should.Throw<Exception>(() => club.DeclineInvite(TouristA))
                .Message.ShouldBe("Invite does not exist.");
        }

        [Fact]
        public void RequestToJoin_succeeds_adds_to_requested()
        {
            var club = CreateClub();

            club.RequestToJoin(TouristA);

            club.RequestedTourists.ShouldContain(TouristA);
        }

        [Fact]
        public void RequestToJoin_fails_when_already_member()
        {
            var club = CreateClub();
            club.AddMember(TouristA);

            Should.Throw<Exception>(() => club.RequestToJoin(TouristA))
                .Message.ShouldBe("Already a member.");
        }

        [Fact]
        public void RequestToJoin_fails_when_request_already_sent()
        {
            var club = CreateClub();
            club.RequestToJoin(TouristA);

            Should.Throw<Exception>(() => club.RequestToJoin(TouristA))
                .Message.ShouldBe("Join request already sent.");
        }

        [Fact]
        public void CancelJoinRequest_succeeds_removes_from_requested()
        {
            var club = CreateClub();
            club.RequestToJoin(TouristA);

            club.CancelJoinRequest(TouristA);

            club.RequestedTourists.ShouldNotContain(TouristA);
        }

        [Fact]
        public void CancelJoinRequest_fails_when_request_does_not_exist()
        {
            var club = CreateClub();

            Should.Throw<Exception>(() => club.CancelJoinRequest(TouristA))
                .Message.ShouldBe("Join request does not exist.");
        }

        [Fact]
        public void AcceptJoinRequest_succeeds_moves_from_requested_to_members()
        {
            var club = CreateClub();
            club.RequestToJoin(TouristA);

            club.AcceptJoinRequest(OwnerId, TouristA);

            club.RequestedTourists.ShouldNotContain(TouristA);
            club.Members.ShouldContain(TouristA);
        }

        [Fact]
        public void AcceptJoinRequest_fails_for_non_owner()
        {
            var club = CreateClub();
            club.RequestToJoin(TouristA);

            Should.Throw<ForbiddenException>(() => club.AcceptJoinRequest(TouristA, TouristA));
        }

        [Fact]
        public void AcceptJoinRequest_fails_when_request_does_not_exist()
        {
            var club = CreateClub();

            Should.Throw<Exception>(() => club.AcceptJoinRequest(OwnerId, TouristA))
                .Message.ShouldBe("Join request does not exist.");
        }

        [Fact]
        public void DeclineJoinRequest_succeeds_removes_from_requested()
        {
            var club = CreateClub();
            club.RequestToJoin(TouristA);

            club.DeclineJoinRequest(OwnerId, TouristA);

            club.RequestedTourists.ShouldNotContain(TouristA);
            club.Members.ShouldNotContain(TouristA);
        }

        [Fact]
        public void DeclineJoinRequest_fails_for_non_owner()
        {
            var club = CreateClub();
            club.RequestToJoin(TouristA);

            Should.Throw<ForbiddenException>(() => club.DeclineJoinRequest(TouristA, TouristA));
        }

        [Fact]
        public void DeclineJoinRequest_fails_when_request_does_not_exist()
        {
            var club = CreateClub();

            Should.Throw<Exception>(() => club.DeclineJoinRequest(OwnerId, TouristA))
                .Message.ShouldBe("Join request does not exist.");
        }
    }
}

using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public enum ClubStatus
    {
        Active,
        Closed
    }

    public class Club : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public long OwnerId { get; init; } //turista
        public List<string> ImageUrls { get; private set; }
        public ClubStatus Status { get; private set; } = ClubStatus.Active;
        public List<long> Members { get; private set; } = new(); // list of tourist IDs
        public List<long> InvitedTourist { get; private set; } = new(); // list of tourist IDs
        public List<long> RequestedTourists { get; private set; } = new(); // list of tourist IDs

        public Club() { }

        public Club(string name, string description, long ownerId, List<string> imageUrls)
        {
            Name = name;
            Description = description;
            OwnerId = ownerId;
            ImageUrls = imageUrls ?? new List<string>();
            Status = ClubStatus.Active;
            Members = new List<long>();
            InvitedTourist = new();
            RequestedTourists = new();
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Invalid Name");

            if (string.IsNullOrWhiteSpace(Description))
                throw new ArgumentException("Invalid Description");

        }

        public void Update(string name, string description, List<string> imageUrls)
        {
            Name = name;
            Description = description;
            ImageUrls = imageUrls ?? new List<string>();
            Validate();
        }

        public void Close(long ownerId)
        {
            if (ownerId != OwnerId)
                throw new ForbiddenException("Only owner can close the club.");

            Status = ClubStatus.Closed;
        }

        public void Activate(long ownerId)
        {
            if (ownerId != OwnerId)
                throw new ForbiddenException("Only owner can activate the club.");

            Status = ClubStatus.Active;
        }
        public void AddMember(long touristId)
        {
            if (!Members.Contains(touristId))
            {
                Members.Add(touristId);
            }
        }
        public void RemoveMember(long ownerId, long touristId)
        {
            if (ownerId != OwnerId)
                throw new ForbiddenException("Only owner can remove members.");

            if (!Members.Contains(touristId))
                throw new Exception("Tourist is not a member.");

            Members.Remove(touristId);
        }
        public void InviteMember(long ownerId, long touristId)
        {
            if (ownerId != OwnerId)
                throw new ForbiddenException("Only owner can invite members.");

            if (Status == ClubStatus.Closed)
                throw new Exception("Cannot invite members to a closed club.");

            if (touristId == OwnerId)
                throw new Exception("Owner cannot be invited.");

            if (Members.Contains(touristId))
                throw new Exception("Tourist is already a member.");

            if (InvitedTourist.Contains(touristId))
                throw new Exception("Invite already sent.");

            InvitedTourist.Add(touristId);
        }

        public void AcceptInvite(long touristId)
        {
            if (Status == ClubStatus.Closed)
                throw new Exception("Cannot accept invite for a closed club.");

            if (!InvitedTourist.Contains(touristId))
                throw new Exception("Invite does not exist.");

            InvitedTourist.Remove(touristId);

            if (!Members.Contains(touristId))
                Members.Add(touristId);
        }

        public void DeclineInvite(long touristId)
        {
            if (!InvitedTourist.Contains(touristId))
                throw new Exception("Invite does not exist.");

            InvitedTourist.Remove(touristId);
        }
        public void RequestToJoin(long touristId)
        {
            if (Members.Contains(touristId))
                throw new Exception("Already a member.");
            if (RequestedTourists.Contains(touristId))
                throw new Exception("Join request already sent.");
            RequestedTourists.Add(touristId);
        }
        public void CancelJoinRequest(long touristId)
        {
            if (!RequestedTourists.Contains(touristId))
                throw new Exception("Join request does not exist.");
            RequestedTourists.Remove(touristId);
        }
        public void AcceptJoinRequest(long ownerId, long touristId)
        {
            if (ownerId != OwnerId)
                throw new ForbiddenException("Only owner can approve join requests.");
            if (!RequestedTourists.Contains(touristId))
                throw new Exception("Join request does not exist.");
            RequestedTourists.Remove(touristId);
            if (!Members.Contains(touristId))
                Members.Add(touristId);
        }
        public void DeclineJoinRequest(long ownerId, long touristId)
        {
            if (ownerId != OwnerId)
                throw new ForbiddenException("Only owner can reject join requests.");
            if (!RequestedTourists.Contains(touristId))
                throw new Exception("Join request does not exist.");
            RequestedTourists.Remove(touristId);
        }
    }
}

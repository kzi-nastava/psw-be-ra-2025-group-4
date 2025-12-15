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
        public List<long> Members { get; private set; } // list of tourist IDs

        public Club() { }

        public Club(string name, string description, long ownerId, List<string> imageUrls)
        {
            Name = name;
            Description = description;
            OwnerId = ownerId;
            ImageUrls = imageUrls ?? new List<string>();
            Status = ClubStatus.Active;
            Members = new List<long>();
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

            if (Members.Contains(touristId))
                throw new Exception("Tourist is already a member.");
        }
    }
}

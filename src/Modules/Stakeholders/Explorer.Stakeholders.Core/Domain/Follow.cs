using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Follow : Entity
    {
        public long FollowerId { get; private set; }
        public User Follower { get; private set; }

        public long FollowedId { get; private set; }
        public User Followed { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private Follow() { }

        public Follow(long followerId, long followedId)
        {
            FollowerId = followerId;
            FollowedId = followedId;
            CreatedAt = DateTime.UtcNow;
            Validate();
        }

        private void Validate()
        {
            if (FollowerId <= 0) throw new ArgumentException("Invalid follower ID");
            if (FollowedId <= 0) throw new ArgumentException("Invalid followed ID");
            if (FollowerId == FollowedId) throw new ArgumentException("User cannot follow himself.");
        }

    }
}


using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class UserProfileDto
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Biography { get; set; }
        public string? Motto { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}

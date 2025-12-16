using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class UpdateUserProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Biography { get; set; }
        public string? Motto { get; set; }
        public string? ProfileImageUrl { get; set; }

        public string? ProfileImageBase64 { get; set; }
    }
}

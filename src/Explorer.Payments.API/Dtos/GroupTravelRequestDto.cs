using System;
using System.Collections.Generic;

namespace Explorer.Payments.API.Dtos
{
    public class GroupTravelRequestDto
    {
        public int Id { get; set; }
        public int OrganizerId { get; set; }
        public string OrganizerUsername { get; set; } = "";
        public int TourId { get; set; }
        public string TourName { get; set; } = "";
        public decimal PricePerPerson { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<GroupTravelParticipantDto> Participants { get; set; } = new List<GroupTravelParticipantDto>();
        public bool CanComplete { get; set; }
    }

    public class GroupTravelParticipantDto
    {
        public int TouristId { get; set; }
        public string TouristUsername { get; set; } = "";
        public int Status { get; set; }
        public DateTime? RespondedAt { get; set; }
    }
}

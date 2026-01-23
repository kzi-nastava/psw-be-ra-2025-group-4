using System.Collections.Generic;

namespace Explorer.Payments.API.Dtos
{
    public class CreateGroupTravelRequestDto
    {
        public int TourId { get; set; }
        public List<string> ParticipantEmails { get; set; } = new List<string>();
    }
}

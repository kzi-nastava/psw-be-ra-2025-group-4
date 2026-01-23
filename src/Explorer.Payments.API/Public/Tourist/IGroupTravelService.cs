using Explorer.Payments.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Payments.API.Public.Tourist
{
    public interface IGroupTravelService
    {
        GroupTravelRequestDto Create(int organizerId, CreateGroupTravelRequestDto dto);
        List<GroupTravelRequestDto> GetMyRequests(int touristId);
        List<GroupTravelRequestDto> GetRequestsForMe(int touristId);
        GroupTravelRequestDto AcceptRequest(int requestId, int participantId);
        GroupTravelRequestDto RejectRequest(int requestId, int participantId);
        List<TourPurchaseTokenDto> CompleteRequest(int requestId, int organizerId);
        void CancelRequest(int requestId, int organizerId);
    }
}

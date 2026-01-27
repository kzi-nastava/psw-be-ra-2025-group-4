using Explorer.Payments.Core.Domain;
using System.Collections.Generic;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IGroupTravelRequestRepository
    {
        GroupTravelRequest? GetById(int id);
        List<GroupTravelRequest> GetByOrganizerId(int organizerId);
        List<GroupTravelRequest> GetByParticipantId(int participantId);
        GroupTravelRequest? GetPendingByParticipantAndTour(int participantId, int tourId);
        GroupTravelRequest Create(GroupTravelRequest request);
        GroupTravelRequest Update(GroupTravelRequest request);
    }
}

using System.Collections.Generic;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IGuideAssignmentRepository
{
    GuideAssignment? GetActiveForExecution(long executionId);
    bool IsGuideBusy(long guideId); 
    GuideAssignment Create(GuideAssignment assignment);
    GuideAssignment Update(GuideAssignment assignment);
}

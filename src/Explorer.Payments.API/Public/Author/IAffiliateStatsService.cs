using System;
using System.Collections.Generic;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Author
{
    public interface IAffiliateStatsService
    {
        AffiliateDashboardSummaryDto GetSummary(int authorId, DateTime? from = null, DateTime? to = null);
        List<AffiliateTourStatsDto> GetByTour(int authorId, DateTime? from = null, DateTime? to = null);
    }
}

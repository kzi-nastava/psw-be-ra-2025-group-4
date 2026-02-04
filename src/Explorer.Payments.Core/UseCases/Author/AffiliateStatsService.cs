using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Author;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases.Author
{
    public class AffiliateStatsService : IAffiliateStatsService
    {
        private readonly IAffiliateRedemptionRepository _repo;

        public AffiliateStatsService(IAffiliateRedemptionRepository repo)
        {
            _repo = repo;
        }

        public AffiliateDashboardSummaryDto GetSummary(int authorId, DateTime? from = null, DateTime? to = null)
        {
            if (authorId == 0) throw new ArgumentException("Invalid authorId.", nameof(authorId));

            var q = Base(authorId, from, to);

            return new AffiliateDashboardSummaryDto
            {
                TotalUsages = q.Count(),
                TotalRevenue = q.Sum(x => (decimal?)x.AmountPaid) ?? 0m,
                TotalCost = q.Sum(x => (decimal?)x.CommissionAmount) ?? 0m
            };
        }

        public List<AffiliateTourStatsDto> GetByTour(int authorId, DateTime? from = null, DateTime? to = null)
        {
            if (authorId == 0) throw new ArgumentException("Invalid authorId.", nameof(authorId));

            var q = Base(authorId, from, to);

            return q
                .GroupBy(x => x.TourId)
                .Select(g => new AffiliateTourStatsDto
                {
                    TourId = g.Key,
                    Usages = g.Count(),
                    Revenue = g.Sum(x => x.AmountPaid),
                    Cost = g.Sum(x => x.CommissionAmount)
                })
     
                .OrderByDescending(x => x.Cost)
                .ThenByDescending(x => x.Usages)
                .ToList();
        }

        private IQueryable<AffiliateRedemption> Base(int authorId, DateTime? from, DateTime? to)
        {

            var q = _repo.Query()
                .Where(x => x.AuthorId == authorId);

            if (from.HasValue)
            {
                q = q.Where(x => x.CreatedAt >= from.Value);
            }

            if (to.HasValue)
            {

                q = q.Where(x => x.CreatedAt < to.Value);
            }

            return q;
        }
    }
}

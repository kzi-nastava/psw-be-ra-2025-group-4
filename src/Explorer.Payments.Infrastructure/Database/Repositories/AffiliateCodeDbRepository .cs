using System.Collections.Generic;
using System.Linq;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class AffiliateCodeDbRepository : IAffiliateCodeRepository
    {
        private readonly PaymentsContext _dbContext;

        public AffiliateCodeDbRepository(PaymentsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AffiliateCode Create(AffiliateCode code)
        {
            _dbContext.AffiliateCodes.Add(code);
            _dbContext.SaveChanges();
            return code;
        }

        public IEnumerable<AffiliateCode> GetByAuthor(int authorId, int? tourId = null)
        {
            var q = _dbContext.AffiliateCodes.Where(x => x.AuthorId == authorId);

            if (tourId.HasValue)
                q = q.Where(x => x.TourId == tourId.Value);

            return q.ToList();
        }

        public bool CodeExists(string code)
        {
            return _dbContext.AffiliateCodes.Any(x => x.Code == code);
        }
    }
}

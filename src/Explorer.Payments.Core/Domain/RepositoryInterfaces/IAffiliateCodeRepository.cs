using System.Collections.Generic;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IAffiliateCodeRepository
    {
        Explorer.Payments.Core.Domain.AffiliateCode Create(Explorer.Payments.Core.Domain.AffiliateCode code);
        IEnumerable<Explorer.Payments.Core.Domain.AffiliateCode> GetByAuthor(int authorId, int? tourId = null);
        bool CodeExists(string code);
    }
}

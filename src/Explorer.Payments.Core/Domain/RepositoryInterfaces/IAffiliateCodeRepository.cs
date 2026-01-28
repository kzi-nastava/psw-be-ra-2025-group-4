using System.Collections.Generic;
using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IAffiliateCodeRepository
    {
        AffiliateCode Create(AffiliateCode code);
        IEnumerable<AffiliateCode> GetByAuthor(int authorId, int? tourId = null);

        AffiliateCode? GetById(int id);
        void SaveChanges();

        bool CodeExists(string code);
        AffiliateCode? GetByCode(string code);

    }
}

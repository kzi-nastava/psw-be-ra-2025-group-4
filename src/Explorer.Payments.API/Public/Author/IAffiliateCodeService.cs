using System.Collections.Generic;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public.Author
{
    public interface IAffiliateCodeService
    {
        AffiliateCodeDto Create(CreateAffiliateCodeDto dto, int authorId);
        List<AffiliateCodeDto> GetAll(int authorId, int? tourId = null);
        void Deactivate(int authorId, int affiliateCodeId);
    }
}

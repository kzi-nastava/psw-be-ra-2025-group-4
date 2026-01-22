using System.Collections.Generic;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ICoinsBundleRepository
    {
        List<CoinsBundle> GetAll();
        CoinsBundle? Get(int id);
        CoinsBundle Create(CoinsBundle bundle);
        CoinsBundle Update(CoinsBundle bundle);
    }
}
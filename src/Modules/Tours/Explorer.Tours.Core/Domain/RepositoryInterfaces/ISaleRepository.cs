using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ISaleRepository
    {
        Sale Create(Sale sale);
        Sale Update(Sale sale);
        void Delete(int id);
        Sale GetById(int id);
        List<Sale> GetByAuthor(int authorId);
        List<Sale> GetActiveSales();
        List<Sale> GetActiveSalesForTour(int tourId);
        List<Sale> GetActiveSalesForTours(List<int> tourIds);
    }
}


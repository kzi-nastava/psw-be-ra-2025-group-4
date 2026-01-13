using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IBundleRepository
    {
        PagedResult<Bundle> GetPaged(int page, int pageSize);
        Bundle GetById(int id);
        Bundle Create(Bundle bundle);
        Bundle Update(Bundle bundle);
        void Delete(int id);
        IEnumerable<Bundle> GetByAuthor(int authorId);
        IEnumerable<Bundle> GetAll();
    }
}


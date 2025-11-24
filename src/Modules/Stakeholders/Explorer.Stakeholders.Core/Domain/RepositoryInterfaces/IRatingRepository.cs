using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IRatingRepository
    {
        Rating GetById(long id);
        List<Rating> GetAll();
        List<Rating> GetByUser(long userId);
        Rating Create(Rating rating);
        Rating Update(Rating rating);
        void Delete(long id);
    }
}

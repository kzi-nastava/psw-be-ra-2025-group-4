using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubRepository
    {
        Club Create(Club club);
        Club GetById(long id);
        IEnumerable<Club> GetAll();
        IEnumerable<Club> GetByOwner(long ownerId);
        Club Update(Club club);
        void Delete(long id);
    }
}

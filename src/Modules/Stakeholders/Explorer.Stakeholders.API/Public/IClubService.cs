using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubService
    {
        ClubDto Create(ClubDto club);
        ClubDto Update(long clubId, ClubDto club);
        void Delete(long clubId, long ownerId);
        List<ClubDto> GetAll();
        List<ClubDto> GetByOwner(long ownerId);
    }
}

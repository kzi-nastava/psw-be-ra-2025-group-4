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
        void InviteMember(long clubId, long ownerId, long touristId);
        void AcceptInvite(long clubId, long touristId);
        void RemoveMember(long clubId, long ownerId, long touristId);

        void CloseClub(long clubId, long ownerId);
        void ActivateClub(long clubId, long ownerId);
    }
}

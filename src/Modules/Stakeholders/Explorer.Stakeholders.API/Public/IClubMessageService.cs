using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubMessageService
    {
        ClubMessageDto Create(long clubId, long authorId, ClubMessageCreateDto dto);
        ClubMessageDto Update(long messageId, long authorId, ClubMessageCreateDto dto);
        void Delete(long messageId, long ownerId);
        List<ClubMessageDto> GetByClub(long clubId);
    }
}

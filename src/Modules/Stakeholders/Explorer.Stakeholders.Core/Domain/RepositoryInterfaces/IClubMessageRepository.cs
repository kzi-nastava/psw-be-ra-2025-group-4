using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubMessageRepository
    {
        ClubMessage Create(ClubMessage message);
        ClubMessage Update(ClubMessage message);
        void Delete(long id);

        ClubMessage GetById(long id);
        ClubMessage GetByClubAndAuthor(long clubId, long authorId);

        IEnumerable<ClubMessage> GetByClub(long clubId);
    }
}

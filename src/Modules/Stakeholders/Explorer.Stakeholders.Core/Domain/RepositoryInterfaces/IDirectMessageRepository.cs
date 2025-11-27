using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IDirectMessageRepository
    {
        PagedResult<DirectMessage> GetPaged(int page, int pageSize, long userId);
        PagedResult<DirectMessage> GetPagedConversations(int page, int pageSize, long userId);
        PagedResult<DirectMessage> GetPagedBetweenUsers(int page, int pageSize, long firstUserId, long secondUserId);

        DirectMessage Create(DirectMessage map);
        DirectMessage Update(DirectMessage map);
        void Delete(long id);
        DirectMessage? Get(long id);
    }
}

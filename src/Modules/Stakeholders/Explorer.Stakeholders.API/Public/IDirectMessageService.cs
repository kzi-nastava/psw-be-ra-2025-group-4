using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Explorer.Stakeholders.API.Public
{
    public interface IDirectMessageService
    {
        PagedResult<DirectMessageDto> GetPaged(int page, int pageSize, string user);
        PagedResult<DirectMessageDto> GetPagedConversations(int page, int pageSize, string user);
        DirectMessageDto SendMessage(DirectMessageDto directMessage);
        DirectMessageDto Update(DirectMessageDto directMessage);
        void Delete(long id);
    }
}

using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Explorer.Stakeholders.API.Public
{
    public interface IDirectMessageService
    {
        PagedResult<DirectMessageDto> GetPaged(int page, int pageSize, long userId);
        PagedResult<DirectMessageDto> GetPagedConversations(int page, int pageSize, long userId);
        PagedResult<DirectMessageDto> GetPagedBetweenUsers(int page, int pageSize, long firstUserId, long secondUserId);
        DirectMessageDto SendMessage(long senderId, DirectMessageDto directMessage);
        DirectMessageDto StartConversation(long senderId, ConversationStartDto directMessage);
        DirectMessageDto Update(DirectMessageDto directMessage);
        void Delete(long id);
        long EnsureConversation(long firstUserId, string username);

    }
}

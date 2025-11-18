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
        PagedResult<DirectMessageDto> GetPaged(int page, int pageSize);
        DirectMessageDto SendMessage(DirectMessageDto directMessage);
        DirectMessageDto Update(DirectMessageDto directMessage);
        void Delete(long id);
    }
}

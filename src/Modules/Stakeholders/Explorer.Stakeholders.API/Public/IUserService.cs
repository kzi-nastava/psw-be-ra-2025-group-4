using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Public
{
    public interface IUserService
    {
        UserDto? GetById(long userId);

    }
}

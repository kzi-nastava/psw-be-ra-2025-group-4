using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Public
{
    public interface IUserProfileService
    {
        UserProfileDto Get(long userId);
        UserProfileDto Update(long userId, UpdateUserProfileDto profile);
    }
}

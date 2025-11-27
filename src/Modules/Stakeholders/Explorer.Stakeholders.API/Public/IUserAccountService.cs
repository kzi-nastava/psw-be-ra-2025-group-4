using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IUserAccountService
    {
        //public bool Update(long userId);

        //public UserAccountDto Create(UserAccountDto userAccountDto);

        public UserAccountDto CreateUser(AccountRegistrationDto account);
        public void BlockUser(long userId);

        public PagedResult<UserAccountDto> GetPaged(int page, int pageSize);
    }
}


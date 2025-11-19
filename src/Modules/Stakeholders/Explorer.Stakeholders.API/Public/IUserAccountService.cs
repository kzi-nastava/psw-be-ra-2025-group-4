using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IUserAccountService
    {
        public bool Update(long userId);

        public UserAccountDto Create(UserAccountDto userAccountDto);

        public AccountRegistrationDto CreateUser(AccountRegistrationDto account);
        public bool BlockUser(long userId);
    }
}

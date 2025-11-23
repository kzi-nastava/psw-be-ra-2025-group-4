using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using System.Security.Principal;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Public;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;

        public UserAccountService(IMapper mapper, IUserRepository userRepository, IPersonRepository personRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _personRepository = personRepository;
        }

        public void BlockUser(long userId)
        {
            var tren = _userRepository.GetById(userId);
            if (tren == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} was not found.");
            }

            var user = _userRepository.GetActiveByName(tren.Username);
            if (user == null)
            {
                // Ako je već blokiran, možemo baciti InvalidOperationException
                throw new InvalidOperationException("User is already blocked or not active.");
            }

            if (user.Role != UserRole.Author && user.Role != UserRole.Tourist)
            {
                throw new InvalidOperationException("Only users with role 'Author' or 'Tourist' can be blocked.");
            }

            user.IsActive = false;
            _userRepository.Update(user);
        }


        public UserAccountDto CreateUser(AccountRegistrationDto account)
        {
            if (_userRepository.Exists(account.Username))
                throw new EntityValidationException("Provided username already exists.");

            if (account.Role.ToLower() != "author" && account.Role.ToLower() != "administrator")
                throw new EntityValidationException("Invalid role provided.");

            var u = new User(account.Username, account.Password, UserRole.Tourist, true); // Temporary role assignment

            var user = _userRepository.Create(new User(account.Username, account.Password, u.GetRole(account.Role), true));
            var person = _personRepository.Create(new Person(user.Id, account.Name, account.Surname, account.Email));

            UserAccountDto temp = new UserAccountDto();
            temp = _mapper.Map<UserAccountDto>(user);
            temp.Email = person.Email;
            return temp;
        }

        public PagedResult<UserAccountDto> GetPaged(int page, int pageSize)
        {
            var result = _personRepository.GetPaged(page, pageSize);

            List<UserAccountDto> finalItems = new List<UserAccountDto>();
            try
            {
                foreach (var person in result.Results)
                {
                    var user = _userRepository.GetById(person.UserId);
                    if (user == null) continue;

                    var account = _mapper.Map<UserAccountDto>(person);
                    account.Username = user.Username;
                    account.Role = user.GetPrimaryRoleName();
                    account.IsActive = user.IsActive;
                    account.Id = user.Id;
                    finalItems.Add(account);
                }

                return new PagedResult<UserAccountDto>(finalItems, result.TotalCount);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("An error occurred while mapping persons to user accounts.", e);
            }
        }

    }
}

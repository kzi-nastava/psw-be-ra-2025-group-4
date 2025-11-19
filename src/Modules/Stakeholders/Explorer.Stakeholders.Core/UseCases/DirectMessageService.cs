using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class DirectMessageService : IDirectMessageService
    {
        private readonly IDirectMessageRepository _directMessageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public DirectMessageService(IUserRepository userRepository, IDirectMessageRepository directMessageRepository, IMapper mapper) 
        {
            _directMessageRepository = directMessageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public DirectMessageDto SendMessage(string senderUsername, DirectMessageDto entity)
        {
            var sender = _userRepository.GetActiveByName(senderUsername);
            if (sender == null)
                throw new ArgumentException($"Sender '{senderUsername}' not found.");

            var recipient = _userRepository.GetActiveByName(entity.Recipient);
            if (recipient == null)
                throw new ArgumentException($"Recipient '{entity.Recipient}' not found.");

            var message = new DirectMessage(sender.Id, recipient.Id, entity.Content, DateTime.UtcNow);
            var result = _directMessageRepository.Create(message);
            return _mapper.Map<DirectMessageDto>(result);
        }


        public void Delete(long id)
        {
            _directMessageRepository.Delete(id);
        }

        public DirectMessageDto Update(DirectMessageDto entity)
        {
            var message = _directMessageRepository.Get(entity.Id);

            if (message == null)
                throw new NotFoundException($"Message with id '{entity.Id}' not found!");

            message.Content = entity.Content;
            message.EditedAt = DateTime.UtcNow;

            var result = _directMessageRepository.Update(message);
            return _mapper.Map<DirectMessageDto>(result);
        }


        public PagedResult<DirectMessageDto> GetPaged(int page, int pageSize, string user)
        {
            var result = _directMessageRepository.GetPaged(page, pageSize, user);

            var items = result.Results.Select(_mapper.Map<DirectMessageDto>).ToList();
            return new PagedResult<DirectMessageDto>(items, result.TotalCount);
        }

        public PagedResult<DirectMessageDto> GetPagedConversations(int page, int pageSize, string user)
        {
            var result = _directMessageRepository.GetPagedConversations(page, pageSize, user);

            var items = result.Results.Select(_mapper.Map<DirectMessageDto>).ToList();
            return new PagedResult<DirectMessageDto>(items, result.TotalCount);
        }
    }
}

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
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;
        private readonly IFollowRepository _followRepository;

        public DirectMessageService(IUserRepository userRepository, IDirectMessageRepository directMessageRepository, IPersonRepository personRepository, IFollowRepository followRepository, IMapper mapper) 
        {
            _directMessageRepository = directMessageRepository;
            _userRepository = userRepository;
            _personRepository = personRepository;
            _followRepository = followRepository;
            _mapper = mapper;
        }

        public DirectMessageDto SendMessage(long senderId, DirectMessageDto entity)
        {
            var sender = _userRepository.Get(senderId)
                ?? throw new ArgumentException($"Sender '{senderId}' not found.");

            var recipient = _userRepository.Get(entity.RecipientId)
                ?? throw new ArgumentException($"Recipient '{entity.RecipientId}' not found.");

            if (!_followRepository.Exists(recipient.Id, sender.Id))
                throw new ArgumentException("You can send a message only to users who follow you.");

            if (string.IsNullOrWhiteSpace(entity.Content))
                throw new ArgumentException("Message content is required.");

            if (entity.Content.Length > 280)
                throw new ArgumentException("Message too long (max 280 chars).");

            long? resourceId = null;
            ResourceType resourceType = ResourceType.None;

            if (!string.IsNullOrWhiteSpace(entity.ResourceUrl))
            {
                if (entity.ResourceUrl.Contains("/blog/"))
                {
                    resourceType = ResourceType.Blog;
                    resourceId = ExtractId(entity.ResourceUrl);
                }
                else if (entity.ResourceUrl.Contains("/tours/"))
                {
                    resourceType = ResourceType.Tour;
                    resourceId = ExtractId(entity.ResourceUrl);
                }
            }

            var message = new DirectMessage(
                sender.Id,
                recipient.Id,
                entity.Content,
                DateTime.UtcNow,
                resourceId,
                resourceType
            );

            var result = _directMessageRepository.Create(message);
            return _mapper.Map<DirectMessageDto>(result);
        }

        private long? ExtractId(string url)
        {
            return long.TryParse(url.Split('/').Last(), out long id) ? id : null;
        }


        public DirectMessageDto StartConversation(long senderId, ConversationStartDto directMessage)
        {
            var sender = _userRepository.Get(senderId);
            if (sender == null)
                throw new ArgumentException($"Sender '{senderId}' not found.");

            var recipient = _userRepository.GetActiveByName(directMessage.Username);
            if (recipient == null)
                throw new ArgumentException($"Recipient '{directMessage.Username}' not found.");

            // ❗ Message allowed ONLY if recipient already follows sender
            if (!_followRepository.Exists(recipient.Id, sender.Id))
                throw new ArgumentException("You can send a message only to users who follow you.");

            if (string.IsNullOrWhiteSpace(directMessage.Content))
                throw new ArgumentException("Message content is required.");

            if (directMessage.Content.Length > 280)
                throw new ArgumentException("Message is too long (max 280 characters).");

            long? resourceId = null;
            ResourceType resourceType = ResourceType.None;

            if (!string.IsNullOrWhiteSpace(directMessage.ResourceUrl))
            {
                if (directMessage.ResourceUrl.Contains("/blog/"))
                {
                    resourceType = ResourceType.Blog;
                    resourceId = ExtractId(directMessage.ResourceUrl);
                }
                else if (directMessage.ResourceUrl.Contains("/tours/"))
                {
                    resourceType = ResourceType.Tour;
                    resourceId = ExtractId(directMessage.ResourceUrl);
                }
            }

            var message = new DirectMessage(
                sender.Id,
                recipient.Id,
                directMessage.Content,
                DateTime.UtcNow,
                resourceId,
                resourceType
            );


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


        public PagedResult<DirectMessageDto> GetPaged(int page, int pageSize, long userId)
        {
            var result = _directMessageRepository.GetPaged(page, pageSize, userId);

            var items = result.Results.Select(_mapper.Map<DirectMessageDto>).ToList();
            return new PagedResult<DirectMessageDto>(items, result.TotalCount);
        }

        public PagedResult<DirectMessageDto> GetPagedConversations(int page, int pageSize, long userId)
        {
            var result = _directMessageRepository.GetPagedConversations(page, pageSize, userId);

            var items = result.Results.Select(_mapper.Map<DirectMessageDto>).ToList();
            return new PagedResult<DirectMessageDto>(items, result.TotalCount);
        }

        public PagedResult<DirectMessageDto> GetPagedBetweenUsers(int page, int pageSize, long firstUserId, long secondUserId)
        {
            var result = _directMessageRepository.GetPagedBetweenUsers(page, pageSize, firstUserId, secondUserId);

            var items = result.Results.Select(_mapper.Map<DirectMessageDto>).ToList();
            return new PagedResult<DirectMessageDto>(items, result.TotalCount);
        }
    }
}

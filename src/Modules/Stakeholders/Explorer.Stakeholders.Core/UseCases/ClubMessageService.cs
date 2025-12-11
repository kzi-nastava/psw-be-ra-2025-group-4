using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubMessageService : IClubMessageService
    {
        private readonly IClubMessageRepository _messageRepo;
        private readonly IClubRepository _clubRepo;
        private readonly IMapper _mapper;

        public ClubMessageService(
            IClubMessageRepository messageRepo,
            IClubRepository clubRepo,
            IMapper mapper)
        {
            _messageRepo = messageRepo;
            _clubRepo = clubRepo;
            _mapper = mapper;
        }

        public ClubMessageDto Create(long clubId, long authorId, ClubMessageCreateDto dto)
        {
            var club = _clubRepo.GetById(clubId);  // ensure club exists

            var existing = _messageRepo.GetByClubAndAuthor(clubId, authorId);
            if (existing != null)
                throw new ForbiddenException("User already posted a message in this club.");

            var message = new ClubMessage(clubId, authorId, dto.Text, dto.ResourceId, dto.ResourceType);
            _messageRepo.Create(message);

            return _mapper.Map<ClubMessageDto>(message);
        }

        public ClubMessageDto Update(long messageId, long authorId, ClubMessageCreateDto dto)
        {
            var message = _messageRepo.GetById(messageId);
            if (message.AuthorId != authorId)
                throw new ForbiddenException("Only the author can update this message.");

            message.SetText(dto.Text);
            _messageRepo.Update(message);

            return _mapper.Map<ClubMessageDto>(message);
        }

        public void Delete(long messageId, long ownerId)
        {
            var message = _messageRepo.GetById(messageId);
            var club = _clubRepo.GetById(message.ClubId);

            if (club.OwnerId != ownerId)
                throw new ForbiddenException("Only club owner can delete messages.");

            _messageRepo.Delete(messageId);
        }

        public List<ClubMessageDto> GetByClub(long clubId)
        {
            var messages = _messageRepo.GetByClub(clubId);
            return _mapper.Map<List<ClubMessageDto>>(messages);
        }
    }
}

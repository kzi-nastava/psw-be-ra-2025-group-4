using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class DirectMessageService : IDirectMessageService
    {
        private readonly IDirectMessageRepository _directMessageRepository;
        private readonly IMapper _mapper;

        public DirectMessageService(IDirectMessageRepository directMessageRepository, IMapper mapper) 
        {
            _directMessageRepository = directMessageRepository;
            _mapper = mapper;
        }

        public DirectMessageDto Create(DirectMessageDto entity)
        {
            var result = _directMessageRepository.Create(_mapper.Map<DirectMessage>(entity));
            return _mapper.Map<DirectMessageDto>(result);
        }

        public void Delete(long id)
        {
            _directMessageRepository.Delete(id);
        }

        public DirectMessageDto Update(DirectMessageDto entity)
        {
            var result = _directMessageRepository.Update(_mapper.Map<DirectMessage>(entity));
            return _mapper.Map<DirectMessageDto>(result);
        }

        public PagedResult<DirectMessageDto> GetPaged(int page, int pageSize)
        {
            var result = _directMessageRepository.GetPaged(page, pageSize);

            var items = result.Results.Select(_mapper.Map<DirectMessageDto>).ToList();
            return new PagedResult<DirectMessageDto>(items, result.TotalCount);
        }
    }
}

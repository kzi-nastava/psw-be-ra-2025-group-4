using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Blog.Core.UseCases
{
    public class DigitalDiaryService : IDigitalDiaryService
    {
        private readonly IDigitalDiaryRepository _repository;
        private readonly IMapper _mapper;

        public DigitalDiaryService(IDigitalDiaryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public DigitalDiaryDto Create(DigitalDiaryDto dto)
        {
            var createdAt = DateTime.UtcNow;
            var status = string.IsNullOrWhiteSpace(dto.Status) ? "Draft" : dto.Status;

            var entity = new DigitalDiary(
                touristId: dto.TouristId,
                title: dto.Title,
                createdAt: createdAt,
                status: status,
                country: dto.Country,
                city: dto.City
            );

            var created = _repository.Create(entity);

            var result = _mapper.Map<DigitalDiaryDto>(created);
            result.TouristId = dto.TouristId;
            result.CreatedAt = createdAt;
            result.Status = status;
            return result;
        }

        public PagedResult<DigitalDiaryDto> GetPagedByTourist(long touristId, int page, int pageSize)
        {
            var pageResult = _repository.GetPagedByTourist(touristId, page, pageSize);
            var items = pageResult.Results.Select(_mapper.Map<DigitalDiaryDto>).ToList();

            foreach (var d in items) d.TouristId = touristId;

            return new PagedResult<DigitalDiaryDto>(items, pageResult.TotalCount);
        }

        public DigitalDiaryDto GetById(long id)
        {
            var entity = _repository.GetById(id) ??
                         throw new InvalidOperationException($"Digital diary {id} not found.");

            var dto = _mapper.Map<DigitalDiaryDto>(entity);
            return dto;
        }

        public DigitalDiaryDto Update(DigitalDiaryDto dto)
        {
            var existing = _repository.GetById(dto.Id) ??
                           throw new NotFoundException("Not found: " + dto.Id);

            if (!string.IsNullOrWhiteSpace(dto.Title) && dto.Title != existing.Title)
                existing.UpdateTitle(dto.Title);

            if (!string.IsNullOrWhiteSpace(dto.Status) && dto.Status != existing.Status)
                existing.ChangeStatus(dto.Status);

            if (!string.IsNullOrWhiteSpace(dto.Country) || dto.City != existing.City)
                existing.UpdateLocation(dto.Country ?? existing.Country, dto.City);

            var saved = _repository.Update(existing);
            var result = _mapper.Map<DigitalDiaryDto>(saved);

            result.TouristId = dto.TouristId;

            return result;
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }
    }
}

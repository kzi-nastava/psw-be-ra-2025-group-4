using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IMapper _mapper;

        public RatingService(IRatingRepository ratingRepository, IMapper mapper)
        {
            _ratingRepository = ratingRepository;
            _mapper = mapper;
        }

        public RatingDto CreateRating(long userId, RatingCreateDto dto)
        {
            var rating = new Rating(
                userId,
                dto.Value,
                dto.Comment
    );
            var created = _ratingRepository.Create(rating);
            return _mapper.Map<RatingDto>(created);
        }

        public RatingDto UpdateRating(long ratingId , long userId, RatingUpdateDto dto)
        {
            var rating = _ratingRepository.GetById(ratingId);
            if (rating == null)
                throw new KeyNotFoundException("Rating not found.");

            if (rating.UserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to update this rating.");

            rating.Update(dto.Value, dto.Comment);

            var updated = _ratingRepository.Update(rating);
            return _mapper.Map<RatingDto>(updated);
        }

        public void DeleteRating(long ratingId, long userId)
        {
            var rating = _ratingRepository.GetById(ratingId);
            if (rating == null)
                throw new KeyNotFoundException("Rating not found.");

            if (rating.UserId != userId)
                throw new UnauthorizedAccessException("User is not allowed to delete this rating.");

            _ratingRepository.Delete(ratingId);
        }

        public List<RatingDto> GetAll()
        {
            var ratings = _ratingRepository.GetAll();
            return _mapper.Map<List<RatingDto>>(ratings);
        }

        public RatingDto GetById(long id)
        {
            var rating = _ratingRepository.GetById(id);
            if (rating == null)
                throw new KeyNotFoundException("Rating not found.");

            return _mapper.Map<RatingDto>(rating);
        }

        public List<RatingDto> GetByUser(long userId)
        {
            var ratings = _ratingRepository.GetByUser(userId);
            return _mapper.Map<List<RatingDto>>(ratings);
        }
    }
}

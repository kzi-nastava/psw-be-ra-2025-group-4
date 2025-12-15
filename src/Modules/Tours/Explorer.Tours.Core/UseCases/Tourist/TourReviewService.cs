using System;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourReviewService : ITourReviewService
    {
        private readonly ITourReviewRepository _repository;
        private readonly ITourExecutionRepository _tourExecutionRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public TourReviewService(ITourReviewRepository repository,
            ITourExecutionRepository tourExecutionRepository,
            ITourRepository tourRepository,
            IMapper mapper)
        {
            _repository = repository;
            _tourExecutionRepository = tourExecutionRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public TourReviewDTO Create(TourReviewDTO tourReviewDto)
        {
            ValidateReviewEligibility(tourReviewDto.TouristId, tourReviewDto.TourId);

 
            var completionPercentage = CalculateCompletionPercentage(tourReviewDto.TouristId, tourReviewDto.TourId);

            var tourReview = new TourReview(
                tourReviewDto.TourId,
                tourReviewDto.TouristId,
                tourReviewDto.Rating,
                tourReviewDto.Comment,
                tourReviewDto.Images,
                DateTime.UtcNow,
                completionPercentage
            );

            var created = _repository.Create(tourReview);
            return _mapper.Map<TourReviewDTO>(created);
        }

        public TourReviewDTO Update(TourReviewDTO tourReviewDto)
        {
            ValidateReviewEligibility(tourReviewDto.TouristId, tourReviewDto.TourId);

            var completionPercentage = CalculateCompletionPercentage(tourReviewDto.TouristId, tourReviewDto.TourId);

            var tourReview = _repository.GetById(tourReviewDto.Id);
            tourReview.Rating = tourReviewDto.Rating;
            tourReview.Comment = tourReviewDto.Comment;
            tourReview.Images = tourReviewDto.Images;
            tourReview.CreatedAt = DateTime.UtcNow;
            tourReview.TourCompletionPercentage = completionPercentage;

            var updated = _repository.Update(tourReview);
            return _mapper.Map<TourReviewDTO>(updated);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public TourReviewDTO GetById(int id)
        {
            var tourReview = _repository.GetById(id);
            return _mapper.Map<TourReviewDTO>(tourReview);
        }

        public PagedResult<TourReviewDTO> GetPagedByTourist(int touristId, int page, int pageSize)
        {
            var all = _repository.GetByTourist(touristId).ToList();
            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mapped = items.Select(_mapper.Map<TourReviewDTO>).ToList();
            return new PagedResult<TourReviewDTO>(mapped, all.Count);
        }

        public PagedResult<TourReviewDTO> GetPagedByTour(int tourId, int page, int pageSize)
        {
            var all = _repository.GetByTour(tourId).ToList();
            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mapped = items.Select(_mapper.Map<TourReviewDTO>).ToList();
            return new PagedResult<TourReviewDTO>(mapped, all.Count);
        }

        public TourReviewDTO GetByTouristAndTour(int touristId, int tourId)
        {
            var tourReview = _repository.GetByTouristAndTour(touristId, tourId);
            return tourReview != null ? _mapper.Map<TourReviewDTO>(tourReview) : null;
        }

        private double CalculateCompletionPercentage(int touristId, int tourId)
        {
            var executions = _tourExecutionRepository.GetByTouristAndTour(touristId, tourId).ToList();

            if (!executions.Any())
                throw new InvalidOperationException("Tourist has not started this tour");

            var lastExecution = executions.OrderByDescending(e => e.LastActivity).First();

            var tour = _tourRepository.GetById(tourId);
            if (tour == null)
                throw new InvalidOperationException("Tour not found");

            var totalPoints = tour.Points.Count;
            if (totalPoints == 0)
                return 0;

            var completedPoints = lastExecution.CompletedPoints.Count;
            return Math.Round((double)completedPoints / totalPoints * 100, 2);
        }

        private void ValidateReviewEligibility(int touristId, int tourId)
        {
            var executions = _tourExecutionRepository.GetByTouristAndTour(touristId, tourId).ToList();

            if (!executions.Any())
                throw new InvalidOperationException("Tourist has not started this tour");

            var lastExecution = executions.OrderByDescending(e => e.LastActivity).First();

            var completionPercentage = CalculateCompletionPercentage(touristId, tourId);

            if (completionPercentage < 35)
                throw new InvalidOperationException("Tourist must complete at least 35% of the tour to leave a review");

            var daysSinceLastActivity = (DateTime.UtcNow - lastExecution.LastActivity).TotalDays;
            if (daysSinceLastActivity > 7)
                throw new InvalidOperationException("More than 7 days have passed since the last activity on this tour");
        }

        public string GetTourAverageGrade(int tourId)
        {
            var allReviews = _repository.GetByTour(tourId).ToList();
            if (!allReviews.Any())
                return "No reviews";

            var averageRating = allReviews.Average(r => r.Rating);
            return averageRating.ToString("0.0");
        }

    }
}
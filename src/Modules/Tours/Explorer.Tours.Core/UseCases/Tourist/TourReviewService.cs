using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourReviewService : ITourReviewService
    {
        private readonly ITourReviewRepository _tourReviewRepository;
        private readonly ITourExecutionRepository _tourExecutionRepository;
        private readonly IMapper _mapper;

        public TourReviewService(
            ITourReviewRepository tourReviewRepository,
            ITourExecutionRepository tourExecutionRepository,
            IMapper mapper)
        {
            _tourReviewRepository = tourReviewRepository;
            _tourExecutionRepository = tourExecutionRepository;
            _mapper = mapper;
        }

        public TourReviewResponseDto CreateReview(int tourId, long touristId, TourReviewCreateDto reviewDto)
        {
            if (_tourReviewRepository.HasReview(touristId, tourId))
            {
                throw new InvalidOperationException("You have already given a review for this tour. Use the update option.");
            }

            var tourExecution = _tourExecutionRepository.GetLastTourExecution(touristId, tourId);

            /*if (tourExecution == null)
            {
                throw new NotFoundException("You did not buy this tour.");
            }*/

            // Ako postoji tourExecution, proveri eligibility (completion, last activity)
            // Ako ne postoji, dozvoli kreiranje review-a (kupovina još nije implementirana)
            if (tourExecution != null)
            {
                if (!tourExecution.CanLeaveReview())
                {
                    var reason = tourExecution.GetReviewIneligibilityReason();
                    throw new InvalidOperationException(reason);
                }
            }

            // Ako nema tourExecution, koristi default vrednost za completion percentage
            double completionPercentage = tourExecution?.CompletionPercentage ?? 0.0;

            var review = new TourReview(
                touristId,
                tourId,
                reviewDto.Rating,
                reviewDto.Comment,
                completionPercentage
            );

            var createdReview = _tourReviewRepository.Create(review);

            return _mapper.Map<TourReviewResponseDto>(createdReview);
        }

        public TourReviewResponseDto UpdateReview(int tourId, long touristId, TourReviewCreateDto reviewDto)
        {
            var existingReview = _tourReviewRepository.GetByTouristAndTour(touristId, tourId);

            if (existingReview == null)
            {
                throw new NotFoundException("You do not have a review for this tour. First create a review.");
            }

            var tourExecution = _tourExecutionRepository.GetLastTourExecution(touristId, tourId);

            // Ako postoji tourExecution, proveri eligibility
            if (tourExecution != null)
            {
                if (!tourExecution.CanLeaveReview())
                {
                    var reason = tourExecution.GetReviewIneligibilityReason();
                    throw new InvalidOperationException($"You cannot update the review: {reason}");
                }
            }

            existingReview.Update(reviewDto.Rating, reviewDto.Comment);

            var updatedReview = _tourReviewRepository.Update(existingReview);

            return _mapper.Map<TourReviewResponseDto>(updatedReview);
        }

        public ReviewEligibilityDto CheckReviewEligibility(int tourId, long touristId)
        {
            var tourExecution = _tourExecutionRepository.GetLastTourExecution(touristId, tourId);

            /*if (tourExecution == null)
            {
                return new ReviewEligibilityDto
                {
                    CanLeaveReview = false,
                    Reason = "You did not buy this tour.",
                    CompletionPercentage = 0,
                    DaysSinceLastActivity = 0,
                    ExistingReview = null
                };
            }*/

            // Ako nema tourExecution, dozvoli review (kupovina nije implementirana)
            if (tourExecution == null)
            {
                var existingReviewWhenNoExecution = _tourReviewRepository.GetByTouristAndTour(touristId, tourId);

                return new ReviewEligibilityDto
                {
                    CanLeaveReview = true,
                    Reason = string.Empty,
                    CompletionPercentage = 0,
                    DaysSinceLastActivity = 0,
                    ExistingReview = existingReviewWhenNoExecution != null
                        ? _mapper.Map<TourReviewResponseDto>(existingReviewWhenNoExecution)
                        : null
                };
            }

            var canLeaveReview = tourExecution.CanLeaveReview();
            var reason = canLeaveReview ? string.Empty : tourExecution.GetReviewIneligibilityReason();

            var daysSinceLastActivity = (int)(DateTime.UtcNow - tourExecution.LastActivity).TotalDays;

            var existingReview = _tourReviewRepository.GetByTouristAndTour(touristId, tourId);

            return new ReviewEligibilityDto
            {
                CanLeaveReview = canLeaveReview,
                Reason = reason,
                CompletionPercentage = tourExecution.CompletionPercentage,
                DaysSinceLastActivity = daysSinceLastActivity,
                ExistingReview = existingReview != null
                    ? _mapper.Map<TourReviewResponseDto>(existingReview)
                    : null
            };
        }

        public TourReviewResponseDto GetReview(int tourId, long touristId)
        {
            var review = _tourReviewRepository.GetByTouristAndTour(touristId, tourId);

            if (review == null)
            {
                throw new NotFoundException("You do not have a review for this tour.");
            }

            return _mapper.Map<TourReviewResponseDto>(review);
        }
    }
}
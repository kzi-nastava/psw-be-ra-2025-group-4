using Explorer.API.Controllers.Tourist.Execution;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourReviewCommandTests : BaseToursIntegrationTest
    {
        public TourReviewCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates_tour_review()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            CleanupExistingReviews(db, -1, -1);
            SetupEligibleExecution(db, -1, -1, 50.0);

            var dto = new TourReviewCreateDto
            {
                Rating = 5,
                Comment = "Amazing tour, would recommend to everyone!"
            };

            var result = ((ObjectResult)controller.CreateReview(-1, dto).Result)?.Value as TourReviewResponseDto;

            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Rating.ShouldBe(5);
            result.Comment.ShouldBe("Amazing tour, would recommend to everyone!");
            result.CompletionPercentage.ShouldBe(50.0);
            result.CreatedAt.ShouldNotBe(default);
            result.LastModifiedAt.ShouldBeNull();

            db.TourReviews.Any(r => r.Id == result.Id && r.TouristId == -1).ShouldBeTrue();
        }

        [Fact]
        public void Create_fails_duplicate_review()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            CleanupExistingReviews(db, -1, -2);
            SetupEligibleExecution(db, -1, -2, 60.0);

            var dto = new TourReviewCreateDto
            {
                Rating = 4,
                Comment = "FIrst review with enough characters."
            };

            service.CreateReview(-2, -1, dto);

            Should.Throw<InvalidOperationException>(() => service.CreateReview(-2, -1, dto));
        }

        /* 
        [Fact]
        public void Create_fails_tour_not_purchased()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();

            var dto = new TourReviewCreateDto
            {
                Rating = 5,
                Comment = "Trying to leave a review without buying the tour."
            };

            Should.Throw<NotFoundException>(() => service.CreateReview(-9999, -1, dto));
        }*/

        [Fact]
        public void Create_fails_completion_below_threshold()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            CleanupExistingReviews(db, -1, -3);
            SetupEligibleExecution(db, -1, -3, 30.0);
            var dto = new TourReviewCreateDto
            {
                Rating = 4,
                Comment = "Trying to leave a review with less than 35% of the tour completed."
            };

            Should.Throw<InvalidOperationException>(() => service.CreateReview(-3, -1, dto));
        }

        [Fact]
        public void Create_fails_last_activity_too_old()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            CleanupExistingReviews(db, -1, -4);
            SetupOldExecution(db, -1, -4, 50.0);

            var dto = new TourReviewCreateDto
            {
                Rating = 3,
                Comment = "Trying to leave a review after more than 7 days."
            };

            Should.Throw<InvalidOperationException>(() => service.CreateReview(-4, -1, dto));
        }

        [Fact]
        public void Updates_tour_review()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            CleanupExistingReviews(db, -1, -5);
            SetupEligibleExecution(db, -1, -5, 70.0);
            CreateReview(scope, -5, -1, 3, "Original comment with enough characters");

            var updateDto = new TourReviewCreateDto
            {
                Rating = 5,
                Comment = "Updated comment after completing the whole tour."
            };

            var result = ((ObjectResult)controller.UpdateReview(-5, updateDto).Result)?.Value as TourReviewResponseDto;

            result.ShouldNotBeNull();
            result.Rating.ShouldBe(5);
            result.Comment.ShouldBe("Updated comment after completing the whole tour.");
            result.LastModifiedAt.ShouldNotBeNull();

            var stored = db.TourReviews.First(r => r.TouristId == -1 && r.TourId == -5);
            stored.Rating.ShouldBe(5);
            stored.LastModifiedAt.ShouldNotBeNull();
        }

        [Fact]
        public void Update_fails_review_not_exists()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            CleanupExistingReviews(db, -1, -6);
            SetupEligibleExecution(db, -1, -6, 50.0);

            var dto = new TourReviewCreateDto
            {
                Rating = 4,
                Comment = "Trying to update a non-excisting review."
            };

            Should.Throw<NotFoundException>(() => service.UpdateReview(-6, -1, dto));
        }

        [Fact]
        public void Update_fails_last_activity_too_old()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            CleanupExistingReviews(db, -1, -7);
            SetupEligibleExecution(db, -1, -7, 50.0);
            CreateReview(scope, -7, -1, 4, "Review I am trying to update");

            var execution = db.TourExecutions.First(te => te.TouristId == -1 && te.TourId == -7);
            typeof(TourExecution).GetProperty("LastActivity")!.SetValue(execution, DateTime.UtcNow.AddDays(-10));
            db.SaveChanges();

            var dto = new TourReviewCreateDto
            {
                Rating = 5,
                Comment = "Trying to update a review after a long time"
            };

            Should.Throw<InvalidOperationException>(() => service.UpdateReview(-7, -1, dto));
        }

        private void CleanupExistingReviews(ToursContext db, long touristId, int tourId)
        {
            var existingReviews = db.TourReviews
                .Where(r => r.TouristId == touristId && r.TourId == tourId)
                .ToList();

            foreach (var review in existingReviews)
            {
                db.TourReviews.Remove(review);
            }
            db.SaveChanges();
        }

        private void SetupEligibleExecution(ToursContext db, long touristId, int tourId, double completionPercentage)
        {
            var existingExecutions = db.TourExecutions
                .Where(te => te.TouristId == touristId && te.TourId == tourId)
                .ToList();

            foreach (var existing in existingExecutions)
            {
                db.TourExecutions.Remove(existing);
            }
            db.SaveChanges();

            var execution = new TourExecution(touristId, tourId, 45.0, 19.0);
            typeof(TourExecution).GetProperty("CompletionPercentage")!.SetValue(execution, completionPercentage);
            typeof(TourExecution).GetProperty("LastActivity")!.SetValue(execution, DateTime.UtcNow.AddHours(-1));

            db.TourExecutions.Add(execution);
            db.SaveChanges();
        }

        private void SetupOldExecution(ToursContext db, long touristId, int tourId, double completionPercentage)
        {
            var execution = new TourExecution(touristId, tourId, 45.0, 19.0);
            typeof(TourExecution).GetProperty("CompletionPercentage")!.SetValue(execution, completionPercentage);
            typeof(TourExecution).GetProperty("LastActivity")!.SetValue(execution, DateTime.UtcNow.AddDays(-10));

            db.TourExecutions.Add(execution);
            db.SaveChanges();
        }

        private void CreateReview(IServiceScope scope, int tourId, long touristId, int rating, string comment)
        {
            var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();
            var dto = new TourReviewCreateDto
            {
                Rating = rating,
                Comment = comment
            };
            service.CreateReview(tourId, touristId, dto);
        }

        private static TourReviewController CreateController(IServiceScope scope)
        {
            return new TourReviewController(scope.ServiceProvider.GetRequiredService<ITourReviewService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}
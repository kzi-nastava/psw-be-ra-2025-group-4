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
    public class TourReviewQueryTests : BaseToursIntegrationTest
    {
        public TourReviewQueryTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_review_by_tour_and_tourist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            CleanupExistingReviews(db, -1, -1);
            SetupEligibleExecution(db, -1, -1, 50.0);
            var created = CreateReview(scope, -1, -1, 5, "Amazing tour with enough characters!");

            var result = ((ObjectResult)controller.GetReview(-1).Result)?.Value as TourReviewResponseDto;

            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.Rating.ShouldBe(5);
            result.Comment.ShouldBe("Amazing tour with enough characters!");
            result.CompletionPercentage.ShouldBe(50.0);
        }

        [Fact]
        public void GetReview_fails_review_not_exists()
        {
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();

            Should.Throw<NotFoundException>(() => service.GetReview(-9999, -1));
        }

        [Fact]
        public void Checks_review_eligibility_when_eligible()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            CleanupExistingReviews(db, -1, -2);
            SetupEligibleExecution(db, -1, -2, 60.0);

            var result = ((ObjectResult)controller.CheckEligibility(-2).Result)?.Value as ReviewEligibilityDto;

            result.ShouldNotBeNull();
            result.CanLeaveReview.ShouldBeTrue();
            result.Reason.ShouldBeEmpty();
            result.CompletionPercentage.ShouldBe(60.0);
            result.DaysSinceLastActivity.ShouldBeLessThanOrEqualTo(1);
            result.ExistingReview.ShouldBeNull();
        }

        [Fact]
        public void Checks_review_eligibility_with_existing_review()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            CleanupExistingReviews(db, -1, -3);
            SetupEligibleExecution(db, -1, -3, 70.0);
            CreateReview(scope, -3, -1, 4, "Excisting review with enough characters");

            var result = ((ObjectResult)controller.CheckEligibility(-3).Result)?.Value as ReviewEligibilityDto;

            result.ShouldNotBeNull();
            result.CanLeaveReview.ShouldBeTrue();
            result.ExistingReview.ShouldNotBeNull();
            result.ExistingReview.Rating.ShouldBe(4);
        }

        [Fact]
        public void Checks_review_eligibility_when_not_eligible_low_completion()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            CleanupExistingReviews(db, -1, -4);
            SetupEligibleExecution(db, -1, -4, 25.0);

            var result = ((ObjectResult)controller.CheckEligibility(-4).Result)?.Value as ReviewEligibilityDto;

            result.ShouldNotBeNull();
            result.CanLeaveReview.ShouldBeFalse();
            result.Reason.ShouldContain("35%");
            result.CompletionPercentage.ShouldBe(25.0);
        }

        /*
         [Fact]
        public void Checks_review_eligibility_when_not_purchased()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = ((ObjectResult)controller.CheckEligibility(-9999).Result)?.Value as ReviewEligibilityDto;

            result.ShouldNotBeNull();
            result.CanLeaveReview.ShouldBeFalse();
            result.Reason.ShouldContain("You did not but this tour");
        }*/

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
            typeof(TourExecution).GetProperty("LastActivity")!.SetValue(execution, System.DateTime.UtcNow.AddHours(-1));

            db.TourExecutions.Add(execution);
            db.SaveChanges();
        }

        private TourReviewResponseDto CreateReview(IServiceScope scope, int tourId, long touristId, int rating, string comment)
        {
            var service = scope.ServiceProvider.GetRequiredService<ITourReviewService>();
            var dto = new TourReviewCreateDto
            {
                Rating = rating,
                Comment = comment
            };
            return service.CreateReview(tourId, touristId, dto);
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
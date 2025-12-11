using System;
using System.Linq;
using System.Collections.Generic;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourReviewCommandTests : BaseToursIntegrationTest
    {
        public TourReviewCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
           
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var newEntity = new TourReviewDTO
            {
                TourId = -1,
                TouristId = -1,
                Rating = 5,
                Comment = "Fantastična tura, preporučujem!",
                Images = new List<string> { "image1.jpg", "image2.jpg" },
                TourCompletionPercentage = 100.0
            };

            var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as TourReviewDTO;

       
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Comment.ShouldBe(newEntity.Comment);
            result.Rating.ShouldBe(newEntity.Rating);
            result.TourId.ShouldBe(newEntity.TourId);
            result.TouristId.ShouldBe(newEntity.TouristId);
            result.TourCompletionPercentage.ShouldBe(100.0);

        
            var storedEntity = dbContext.TourReviews.FirstOrDefault(tr => tr.Comment == newEntity.Comment);
            storedEntity.ShouldNotBeNull();
            storedEntity.Id.ShouldBe(result.Id);
        }

        [Fact]
        public void Create_fails_invalid_rating()
        {
          
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var invalidEntity = new TourReviewDTO
            {
                TourId = -1,
                TouristId = -1,
                Rating = 6,
                Comment = "Test komentar",
                Images = new List<string>(),
                TourCompletionPercentage = 50.0
            };

            Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
        }

        [Fact]
        public void Create_fails_empty_comment()
        {
         
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var invalidEntity = new TourReviewDTO
            {
                TourId = -1,
                TouristId = -1,
                Rating = 4,
                Comment = "",
                Images = new List<string>(),
                TourCompletionPercentage = 50.0
            };

       
            Should.Throw<ArgumentException>(() => controller.Create(invalidEntity));
        }

        [Fact]
        public void Create_fails_insufficient_completion()
        {
            
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var invalidEntity = new TourReviewDTO
            {
                TourId = -2,  
                TouristId = -1,
                Rating = 5,
                Comment = "Test komentar",
                Images = new List<string>(),
                TourCompletionPercentage = 20.0
            };

            
            Should.Throw<InvalidOperationException>(() => controller.Create(invalidEntity));
        }

        [Fact]
        public void Updates()
        {
           
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var updatedEntity = new TourReviewDTO
            {
                Id = -1,
                TourId = -1,
                TouristId = -1,
                Rating = 4,
                Comment = "Ažuriran komentar o turi",
                Images = new List<string> { "new_image.jpg" },
                TourCompletionPercentage = 100.0
            };

            
            var result = ((ObjectResult)controller.Update(-1, updatedEntity).Result)?.Value as TourReviewDTO;

          
            result.ShouldNotBeNull();
            result.Id.ShouldBe(-1);
            result.Comment.ShouldBe(updatedEntity.Comment);
            result.Rating.ShouldBe(4);

           
            var storedEntity = dbContext.TourReviews.FirstOrDefault(tr => tr.Id == -1);
            storedEntity.ShouldNotBeNull();
            storedEntity.Comment.ShouldBe(updatedEntity.Comment);
            storedEntity.Rating.ShouldBe(4);
        }

        [Fact]
        public void Update_fails_invalid_id()
        {
           
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var updatedEntity = new TourReviewDTO
            {
                Id = -1000,
                TourId = -1,
                TouristId = -1,
                Rating = 5,
                Comment = "Test komentar",
                Images = new List<string>(),
                TourCompletionPercentage = 100.0
            };

           
            Should.Throw<NotFoundException>(() => controller.Update(-1000, updatedEntity));
        }

        [Fact]
        public void Deletes()
        {
          
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            
            var result = (NoContentResult)controller.Delete(-3);

            
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(204);

            
            var storedEntity = dbContext.TourReviews.FirstOrDefault(tr => tr.Id == -3);
            storedEntity.ShouldBeNull();
        }

        [Fact]
        public void Delete_fails_invalid_id()
        {
            
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

          
            Should.Throw<NotFoundException>(() => controller.Delete(-1000));
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
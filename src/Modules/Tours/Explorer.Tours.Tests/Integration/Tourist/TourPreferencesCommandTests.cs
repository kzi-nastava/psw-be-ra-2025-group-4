using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourPreferencesCommandTests : BaseToursIntegrationTest
    {
        public TourPreferencesCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Gets_preferences_for_existing_tourist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "2"); 

            var actionResult = controller.Get();

            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var dto = okResult.Value as TourPreferencesDTO;
            dto.ShouldNotBeNull();

            
        }

        [Fact]
        public void Get_returns_not_found_when_preferences_missing()
        {
            using var scope = Factory.Services.CreateScope();
            
            var controller = CreateController(scope, "999");

            var actionResult = controller.Get();

            actionResult.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void Save_updates_existing_preferences_and_persists_to_db()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "2");
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var dto = new TourPreferencesDTO
            {
                
                PreferredDifficulty = "Hard",
                WalkRating = 0,
                BikeRating = 2,
                CarRating = 3,
                BoatRating = 1,
                Tags = new List<string> { "avantura", "planina" }
            };

            var actionResult = controller.Save(dto);

            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var resultDto = okResult.Value as TourPreferencesDTO;
            resultDto.ShouldNotBeNull();

            
            resultDto.Id.ShouldBe(-1);
            resultDto.PreferredDifficulty.ShouldBe(dto.PreferredDifficulty);
            resultDto.WalkRating.ShouldBe(dto.WalkRating);
            resultDto.BikeRating.ShouldBe(dto.BikeRating);
            resultDto.CarRating.ShouldBe(dto.CarRating);
            resultDto.BoatRating.ShouldBe(dto.BoatRating);
            resultDto.Tags.ShouldBe(dto.Tags);

            
            var stored = dbContext.TourPreferences.AsNoTracking()
                .FirstOrDefault(tp => tp.Id == resultDto.Id);

            stored.ShouldNotBeNull();
            stored.TouristId.ShouldBe(2);
            stored.PreferredDifficulty.ToString().ShouldBe(dto.PreferredDifficulty);
            stored.WalkRating.ShouldBe(dto.WalkRating);
            stored.BikeRating.ShouldBe(dto.BikeRating);
            stored.CarRating.ShouldBe(dto.CarRating);
            stored.BoatRating.ShouldBe(dto.BoatRating);
            stored.Tags.ShouldBe(dto.Tags);
        }

        [Fact]
        public void Save_fails_when_ratings_out_of_range()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "2");

            var invalidDto = new TourPreferencesDTO
            {
                PreferredDifficulty = "Easy",
                WalkRating = 5, 
                BikeRating = 0,
                CarRating = 0,
                BoatRating = 0,
                Tags = new List<string>()
            };

            Should.Throw<ArgumentException>(() => controller.Save(invalidDto));
        }

        [Fact]
        public void Deletes_preferences_for_existing_tourist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "3");
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var result = controller.Delete() as NoContentResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(204);

            var stored = dbContext.TourPreferences.AsNoTracking()
                .FirstOrDefault(tp => tp.TouristId == 3);

            stored.ShouldBeNull();
        }

        [Fact]
        public void Delete_when_preferences_not_found_returns_no_content()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "999"); 

            var result = controller.Delete() as NoContentResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(204);
        }


        private static TourPreferencesController CreateController(IServiceScope scope, string personId)
        {
            return new TourPreferencesController(
                scope.ServiceProvider.GetRequiredService<ITourPreferencesService>())
            {
                ControllerContext = BuildContext(personId)
            };
        }
    }
}

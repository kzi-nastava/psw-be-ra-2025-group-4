using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TourPreferencesQueryTests : BaseToursIntegrationTest
    {
        public TourPreferencesQueryTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Gets_preferences_for_existing_tourist()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "2");  

            var result = ((ObjectResult)controller.Get().Result)?.Value as TourPreferencesDTO;

            result.ShouldNotBeNull();
            
        }

        [Fact]
        public void Get_returns_not_found_for_tourist_without_preferences()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "999"); 

            var actionResult = controller.Get();

            actionResult.Result.ShouldBeOfType<NotFoundResult>();
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

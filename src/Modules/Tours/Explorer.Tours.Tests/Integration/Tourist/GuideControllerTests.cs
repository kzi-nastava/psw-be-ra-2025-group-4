using System.Linq;
using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class GuideControllerTests : BaseToursIntegrationTest
    {
        public GuideControllerTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Get_available_select_and_cancel_guide()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var execution = new TourExecution(touristId: -21, tourId: -2, startLatitude: 0, startLongitude: 0);
            db.TourExecutions.Add(execution);
            db.SaveChanges();

            var guide = new Guide("Test Guide", new[] { "SR" }, 10);
            db.Guides.Add(guide);
            db.SaveChanges();

            db.GuideTours.Add(new GuideTour(guide.Id, -2));
            db.SaveChanges();

            var controller = new GuideController(scope.ServiceProvider.GetRequiredService<IGuideSelectionService>())
            {
                ControllerContext = BuildContext("-21")
            };

            var available = controller.GetAvailable(-2, execution.Id) as OkObjectResult;
            available.ShouldNotBeNull();
            var list = available!.Value as System.Collections.Generic.IEnumerable<GuideDto>;
            list.ShouldNotBeNull();
            list!.Any(g => g.Id == guide.Id).ShouldBeTrue();

            var selectResult = controller.SelectGuide(execution.Id, new SelectGuideDto { GuideId = guide.Id }) as NoContentResult;
            selectResult.ShouldNotBeNull();

            var selected = controller.GetSelectedGuide(execution.Id) as OkObjectResult;
            selected.ShouldNotBeNull();
            var selectedDto = selected!.Value as GuideDto;
            selectedDto.ShouldNotBeNull();
            selectedDto!.Id.ShouldBe(guide.Id);

            var cancelResult = controller.CancelGuide(execution.Id) as NoContentResult;
            cancelResult.ShouldNotBeNull();
        }
    }
}

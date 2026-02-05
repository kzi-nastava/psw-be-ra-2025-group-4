using System.Linq;
using Explorer.API.Controllers.Author;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Integration.Author
{
    [Collection("Sequential")]
    public class AuthorTouristsControllerTests : BaseStakeholdersIntegrationTest
    {
        public AuthorTouristsControllerTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public void GetAll_returns_active_tourists()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = new AuthorTouristsController(scope.ServiceProvider.GetRequiredService<ITouristLookupService>());

            var action = controller.GetAll(true);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();

            var tourists = result!.Value as List<TouristLookupDto>;
            tourists.ShouldNotBeNull();
            tourists.Count.ShouldBe(3);
            tourists.Select(t => t.Id).ShouldContain(-21);
            tourists.Select(t => t.Id).ShouldContain(-22);
            tourists.Select(t => t.Id).ShouldContain(-23);
        }
    }
}

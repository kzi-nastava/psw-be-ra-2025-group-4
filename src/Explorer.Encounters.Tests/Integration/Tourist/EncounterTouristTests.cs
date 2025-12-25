using Explorer.API.Controllers.Administrator.Administration;
using Explorer.API.Controllers.Tourist.Encounters;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Administration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class EncounterTouristTests : BaseEncountersIntegrationTest
    {
        public EncounterTouristTests(EncountersTestFactory factory) : base(factory) { }

        [Fact]
        public void Retrieves_paged_encounters()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = ((ObjectResult)controller.GetActive().Result)?.Value as IEnumerable<EncounterDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
        }

        private static TouristEncountersController CreateController(IServiceScope scope)
        {
            return new TouristEncountersController(
                scope.ServiceProvider.GetRequiredService<IEncounterService>())
            {
                ControllerContext = BuildContext("-21")
            };
        }
    }
}

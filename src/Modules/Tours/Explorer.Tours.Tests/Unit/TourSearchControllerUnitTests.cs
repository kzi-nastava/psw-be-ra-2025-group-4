using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.API.Controllers.Tours;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class TourSearchControllerUnitTests
    {
        private class SearchServiceStub : ITourSearchService
        {
            public List<TourSearchResultDto> Result { get; set; } = new();
            public List<TourSearchResultDto> Search(TourSearchRequestDto request) => Result;
        }

        [Fact]
        public void Search_returns_422_for_invalid_request()
        {
            var svc = new SearchServiceStub();
            var controller = new TourSearchController(svc);

            var res = controller.Search(null);
            res.Result.ShouldBeOfType<UnprocessableEntityObjectResult>();

            var badReq = new TourSearchRequestDto { Lat = 100, Lon = 0, RadiusKm = 1 };
            res = controller.Search(badReq);
            res.Result.ShouldBeOfType<UnprocessableEntityObjectResult>();

            badReq = new TourSearchRequestDto { Lat = 0, Lon = 0, RadiusKm = 0 };
            res = controller.Search(badReq);
            res.Result.ShouldBeOfType<UnprocessableEntityObjectResult>();
        }

        [Fact]
        public void Search_returns_ok_for_valid_request()
        {
            var svc = new SearchServiceStub();
            svc.Result.Add(new TourSearchResultDto { TourId = 1, Name = "T1" });

            var controller = new TourSearchController(svc);

            var req = new TourSearchRequestDto { Lat = 45, Lon = 19, RadiusKm = 5 };
            var action = controller.Search(req);

            action.Result.ShouldBeOfType<OkObjectResult>();
            var ok = action.Result as OkObjectResult;
            var list = ok!.Value as List<TourSearchResultDto>;
            list.ShouldNotBeNull();
            list.Count.ShouldBe(1);
            list[0].Name.ShouldBe("T1");
        }
    }
}

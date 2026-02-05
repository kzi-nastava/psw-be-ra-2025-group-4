using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Explorer.API.Controllers;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Stakeholders.API.Public;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class PublicTourControllerTests
    {
        private class TourServiceStub : ITourService
        {
            public TourDto ById { get; set; }
            public List<string> Tags { get; set; } = new();
            public PagedResult<TourDto> PublishedFilteredResult { get; set; } = new(new List<TourDto>(), 0);

            public PagedResult<TourDto> GetPagedByAuthor(int authorId, int page, int pageSize) => throw new System.NotImplementedException();
            public TourDto GetByIdForAuthor(int authorId, int id) => throw new System.NotImplementedException();
            public TourDto GetById(int id) => ById;
            public TourDto Create(CreateUpdateTourDto dto, int authorId) => throw new System.NotImplementedException();
            public TourDto Update(int id, CreateUpdateTourDto dto, int authorId) => throw new System.NotImplementedException();
            public void DeleteForAuthor(int authorId, int id) => throw new System.NotImplementedException();
            public void Publish(int tourId, int authorId) => throw new System.NotImplementedException();
            public void Archive(int tourId, int authorId) => throw new System.NotImplementedException();
            public void SetPrice(int tourId, int authorId, decimal price) => throw new System.NotImplementedException();
            public void AddEquipment(int tourId, int authorId, List<EquipmentDto> equipment) => throw new System.NotImplementedException();
            public void AddTourPoint(int tourId, int authorId, TourPointDto tourPoint) => throw new System.NotImplementedException();
            public PagedResult<TourDto> GetPublishedAndArchived(int page, int pageSize) => throw new System.NotImplementedException();
            public TourDto UpdateRouteLength(int tourId, int authorId, double lengthInKm) => throw new System.NotImplementedException();
            public PagedResult<TourDto> GetPublished(int page, int pageSize) => throw new System.NotImplementedException();
            public PagedResult<TourDto> GetPublishedFiltered(int page, int pageSize, string? search, int? difficulty, decimal? minPrice, decimal? maxPrice, List<string>? tags, string? sort, bool? onSale)
                => PublishedFilteredResult;
            public IEnumerable<string> GetAllTags() => Tags;
            public PagedResult<PopularTourDto> GetPopular(int authorId, int page, int pageSize, double? lat, double? lon, double? radiusKm) => throw new System.NotImplementedException();
            public PagedResult<AuthorTourDashboardItemDto> GetDashboard(int authorId, int page, int pageSize) => throw new System.NotImplementedException();
            public AuthorTourDashboardDetailsDto GetDashboardDetails(int authorId, int tourId, int days = 30) => throw new System.NotImplementedException();
        }

        private class ReviewServiceStub : ITourReviewService
        {
            public TourReviewDTO Create(TourReviewDTO tourReviewDto) => throw new System.NotImplementedException();
            public TourReviewDTO Update(TourReviewDTO tourReviewDto) => throw new System.NotImplementedException();
            public void Delete(int id) => throw new System.NotImplementedException();
            public TourReviewDTO GetById(int id) => throw new System.NotImplementedException();
            public PagedResult<TourReviewDTO> GetPagedByTourist(int touristId, int page, int pageSize) => throw new System.NotImplementedException();
            public PagedResult<TourReviewDTO> GetPagedByTour(int tourId, int page, int pageSize) => throw new System.NotImplementedException();
            public TourReviewDTO GetByTouristAndTour(int touristId, int tourId) => throw new System.NotImplementedException();
            public string GetTourAverageGrade(int tourId) => "4.5";
            public ReviewEligibilityInfo GetReviewEligibilityInfo(int touristId, int tourId) => new();
        }

        [Fact]
        public void GetById_returns_tour_with_average_grade()
        {
            var tour = new TourDto { Id = 1, Name = "T", Description = "D" };
            var svc = new TourServiceStub { ById = tour };
            var rev = new ReviewServiceStub();
            var controller = new PublicTourController(svc, rev);

            var action = controller.GetById(1);
            var result = action.Result as OkObjectResult;
            result.ShouldNotBeNull();
            var dto = result!.Value as TourDto;
            dto.ShouldNotBeNull();
            dto.Id.ShouldBe(1);
            dto.AverageGrade.ShouldBe("4.5");
        }

        [Fact]
        public void GetAll_returns_paged_result_and_sets_average_grade_for_each()
        {
            var t1 = new TourDto { Id = 1, Name = "T1" };
            var svc = new TourServiceStub { PublishedFilteredResult = new PagedResult<TourDto>(new List<TourDto> { t1 }, 1) };
            var rev = new ReviewServiceStub();
            var controller = new PublicTourController(svc, rev);

            var action = controller.GetAll(1, 10, null, null, null, null, null, null, null);
            var res = action.Result as OkObjectResult;
            res.ShouldNotBeNull();
            var page = res!.Value as PagedResult<TourDto>;
            page.ShouldNotBeNull();
            page.Results.Count.ShouldBe(1);
            page.Results[0].AverageGrade.ShouldBe("4.5");
        }

        [Fact]
        public void GetTags_returns_list()
        {
            var svc = new TourServiceStub { Tags = new List<string> { "a", "b" } };
            var controller = new PublicTourController(svc, new ReviewServiceStub());

            var action = controller.GetTags();
            var res = action.Result as OkObjectResult;
            res.ShouldNotBeNull();
            var tags = res!.Value as IEnumerable<string>;
            tags.ShouldNotBeNull();
        }
    }
}

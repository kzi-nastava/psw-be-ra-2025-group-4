using Explorer.Tours.API.Dtos;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class UpdateBundleDtoTests
    {
        [Fact]
        public void Can_set_and_get_properties()
        {
            var dto = new UpdateBundleDto
            {
                Name = "New name",
                Price = 123.45m
            };

            dto.Name.ShouldBe("New name");
            dto.Price.ShouldBe(123.45m);

            // pokriva i default init liste (new List<int>())
            dto.TourIds.ShouldNotBeNull();
            dto.TourIds.ShouldBeEmpty();

            dto.TourIds.AddRange(new[] { 1, 2, 3 });
            dto.TourIds.Count.ShouldBe(3);
            dto.TourIds.ShouldContain(2);
        }
    }
}
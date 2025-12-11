using System.Collections.Generic;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITourSearchService
    {
        List<TourSearchResultDto> Search(TourSearchRequestDto request);
    }
}
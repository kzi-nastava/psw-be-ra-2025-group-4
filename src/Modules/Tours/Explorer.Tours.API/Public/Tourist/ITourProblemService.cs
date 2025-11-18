using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITourProblemService
    {
        TourProblemDto Create(TourProblemDto tourProblemDto);
        TourProblemDto Update(TourProblemDto tourProblemDto);
        void Delete(int id);
        TourProblemDto GetById(int id);
        PagedResult<TourProblemDto> GetPagedByTourist(int touristId, int page, int pageSize);
    }
}
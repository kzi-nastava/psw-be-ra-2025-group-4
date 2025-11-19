using Explorer.Blog.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Public
{
    public interface IDigitalDiaryService
    {
        DigitalDiaryDto Create(DigitalDiaryDto dto);
        DigitalDiaryDto Update(DigitalDiaryDto dto);
        void Delete(long id);
        DigitalDiaryDto GetById(long id);
        PagedResult<DigitalDiaryDto> GetPagedByTourist(long touristId, int page, int pageSize);

    }
}

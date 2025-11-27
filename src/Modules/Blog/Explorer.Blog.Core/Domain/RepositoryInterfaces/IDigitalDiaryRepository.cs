using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces
{
    public interface IDigitalDiaryRepository
    {
        DigitalDiary Create(DigitalDiary diary);
        DigitalDiary Update(DigitalDiary diary);
        void Delete(long id);
        DigitalDiary? GetById(long id);
        PagedResult<DigitalDiary> GetPagedByTourist(long touristId, int page, int pageSize);
    }
}

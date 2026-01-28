using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface ITouristLookupService
    {
        List<TouristLookupDto> GetAll(bool onlyActive = true);
    }
}

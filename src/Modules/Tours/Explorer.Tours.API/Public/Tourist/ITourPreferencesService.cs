using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITourPreferencesService
    {
        TourPreferencesDTO? GetForTourist(int touristId);
        TourPreferencesDTO CreateOrUpdate(int touristId, TourPreferencesDTO dto);
        void Delete(int touristId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourPreferencesRepository
    {
        TourPreferences? GetByTouristId(int touristId);
        TourPreferences Create(TourPreferences preferences);
        TourPreferences Update(TourPreferences preferences);
        void DeleteByTouristId(int touristId);
    }
}

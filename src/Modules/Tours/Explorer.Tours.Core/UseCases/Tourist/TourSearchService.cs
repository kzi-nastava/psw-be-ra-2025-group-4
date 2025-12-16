using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourSearchService : ITourSearchService
    {
        private readonly ITourRepository _tourRepo;

        public TourSearchService(ITourRepository tourRepo)
        {
            _tourRepo = tourRepo;
        }

        public List<TourSearchResultDto> Search(TourSearchRequestDto req)
        {
            if (req.RadiusKm <= 0) return new();

            var tours = _tourRepo.GetPublished().ToList();

            double dLat = req.RadiusKm / 111.0;
            double dLon = req.RadiusKm / (Math.Cos(req.Lat * Math.PI / 180.0) * 111.0);

            var results = new List<TourSearchResultDto>();

            foreach (var t in tours)
            {
                var candidates = t.Points.Where(kp =>
                    kp.Latitude >= req.Lat - dLat && kp.Latitude <= req.Lat + dLat &&
                    kp.Longitude >= req.Lon - dLon && kp.Longitude <= req.Lon + dLon
                );

                var nearest = candidates
                    .Select(kp => new { kp, dist = HaversineKm(req.Lat, req.Lon, kp.Latitude, kp.Longitude) })
                    .Where(x => x.dist <= req.RadiusKm + 1e-6)
                    .OrderBy(x => x.dist)
                    .FirstOrDefault();

                if (nearest != null)
                {
                    results.Add(new TourSearchResultDto
                    {
                        TourId = (int)t.Id,
                        Name = t.Name,
                        ShortDescription = string.IsNullOrWhiteSpace(t.Description)
                            ? ""
                            : (t.Description.Length > 140 ? t.Description[..140] + "..." : t.Description),
                        MatchingPoint = new MatchingPointDto
                        {
                            KeyPointId = (int)nearest.kp.Id,
                            Title = nearest.kp.Name,
                            Lat = nearest.kp.Latitude,
                            Lon = nearest.kp.Longitude,
                            DistanceKm = Math.Round(nearest.dist, 3)
                        }
                    });
                }
            }

            return results.OrderBy(r => r.MatchingPoint.DistanceKm).ToList();
        }

        private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0088; // km
            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);
            lat1 = ToRad(lat1);
            lat2 = ToRad(lat2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double ToRad(double deg) => deg * (Math.PI / 180.0);
    }
}
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Author
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly ITourReviewRepository _tourReviewRepository;
        private readonly ITourExecutionRepository _tourExecutionRepository;
        private readonly IMapper _mapper;

        public TourService(ITourRepository tourRepository, ITourReviewRepository tourReviewRepository, ITourExecutionRepository tourExecutionRepository, IMapper mapper)
        {
            _tourRepository = tourRepository;
            _tourReviewRepository = tourReviewRepository;
            _tourExecutionRepository = tourExecutionRepository;
            _mapper = mapper;
        }

        public PagedResult<TourDto> GetPagedByAuthor(int authorId, int page, int pageSize)
        {
            var all = _tourRepository.GetByAuthor(authorId)
                                     .OrderBy(t => t.Id)
                                     .ToList();

            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mapped = items.Select(_mapper.Map<TourDto>).ToList();
            return new PagedResult<TourDto>(mapped, all.Count);
        }

        public TourDto GetByIdForAuthor(int authorId, int id)
        {
            var tour = _tourRepository.GetById(id);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("Not your tour.");

            var dto = _mapper.Map<TourDto>(tour);

            if (dto.Points != null)
            {
                dto.Points = dto.Points
                    .OrderBy(p => p.Order)
                    .ToList();
            }

            return dto;
        }

        public TourDto GetById(int id)
        {
            var tour = _tourRepository.GetById(id);

            var dto = _mapper.Map<TourDto>(tour);

            if (dto.Points != null)
            {
                dto.Points = dto.Points
                    .OrderBy(p => p.Order)
                    .ToList();
            }

            return dto;
        }

        public TourDto Create(CreateUpdateTourDto dto, int authorId)
        {
            var durations = dto.TransportDuration.Select(_mapper.Map<TourTransportDuration>).ToList();
            var tour = new Tour(dto.Name, dto.Description,
                (TourDifficulty)dto.Difficulty, authorId, durations, dto.Tags);

            var created = _tourRepository.Create(tour);

            return GetByIdForAuthor(authorId, (int)created.Id);
        }

        public TourDto Update(int id, CreateUpdateTourDto dto, int authorId)
        {
            var tour = _tourRepository.GetById(id);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("Not your tour.");

            var durations = dto.TransportDuration.Select(_mapper.Map<TourTransportDuration>).ToList();

            tour.Update(dto.Name, dto.Description,
                (TourDifficulty)dto.Difficulty, durations, dto.Tags);

            _tourRepository.Update(tour);

            return GetByIdForAuthor(authorId, id);
        }

        public void DeleteForAuthor(int authorId, int id)
        {
            var tour = _tourRepository.GetById(id);

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("Not your tour.");

            if (tour.Status != TourStatus.Draft)
                throw new ForbiddenException("Only draft tours can be deleted.");

            _tourRepository.Delete(id);
        }

        public void Publish(int tourId, int authorId)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");
            tour.Publish();
            _tourRepository.Update(tour);
        }

        public void Archive(int tourId, int authorId)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");
            tour.Archive();
            _tourRepository.Update(tour);
        }

        public void SetPrice(int tourId, int authorId, decimal price)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");
            tour.SetPrice(price);
            _tourRepository.Update(tour);
        }

        public void AddEquipment(int tourId, int authorId, List<EquipmentDto> equipment)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");
            var equipmentMap = equipment.Select(_mapper.Map<Equipment>).ToList();
            tour.AddEquipments(equipmentMap);
            _tourRepository.Update(tour);
        }

        public void AddTourPoint(int tourId, int authorId, TourPointDto tourPoint)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");
            tour.AddTourPoint(_mapper.Map<TourPoint>(tourPoint));
            _tourRepository.Update(tour);
        }

        public PagedResult<TourDto> GetPublishedAndArchived(int page, int pageSize)
        {
            var all = _tourRepository.GetPublishedAndArchived()
                                     .OrderBy(t => t.Id)
                                     .ToList();

            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mapped = items.Select(t =>
            {
                var dto = _mapper.Map<TourDto>(t);
                if (dto.Points != null)
                {
                    dto.Points = dto.Points.OrderBy(p => p.Order).ToList();
                }
                return dto;
            }).ToList();

            return new PagedResult<TourDto>(mapped, all.Count);
        }

        public TourDto UpdateRouteLength(int tourId, int authorId, double lengthInKm)
        {
            var tour = _tourRepository.GetById(tourId) ?? throw new KeyNotFoundException($"Tour {tourId} not found.");

            if (tour.AuthorId != authorId) throw new ForbiddenException("Not your tour.");

            tour.SetLengthFromRoute(lengthInKm);
            _tourRepository.Update(tour);

            return _mapper.Map<TourDto>(tour);
        }

        public PagedResult<TourDto> GetPublished(int page, int pageSize)
        {
            var all = _tourRepository.GetPublished()
                                     .OrderBy(t => t.Id)
                                     .ToList();

            var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var mapped = items.Select(t =>
            {
                var dto = _mapper.Map<TourDto>(t);
                if (dto.Points != null)
                {
                    dto.Points = dto.Points.OrderBy(p => p.Order).ToList();
                }
                return dto;
            }).ToList();

            return new PagedResult<TourDto>(mapped, all.Count);
        }

        public PagedResult<TourDto> GetPublishedFiltered(
            int page, int pageSize,
            string? search,
            int? difficulty,
            decimal? minPrice, decimal? maxPrice,
            List<string>? tags,
            string? sort)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var q = _tourRepository.QueryPublished();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                q = q.Where(t => t.Name.ToLower().Contains(s));
            }

            if (difficulty.HasValue && difficulty.Value >= 0 && difficulty.Value <= 2)
            {
                var diff = (TourDifficulty)difficulty.Value;
                q = q.Where(t => t.Difficulty == diff);
            }

            if (minPrice.HasValue)
                q = q.Where(t => t.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                q = q.Where(t => t.Price <= maxPrice.Value);

            if (tags != null && tags.Count > 0)
            {
                var wanted = tags
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .ToList();

                if (wanted.Count > 0)
                {
                    q = q.Where(t => t.Tags != null && t.Tags.Any(tag => wanted.Contains(tag)));
                }
            }

            q = (sort ?? "").Trim().ToLower() switch
            {
                "nameasc" => q.OrderBy(t => t.Name),
                "namedesc" => q.OrderByDescending(t => t.Name),
                "priceasc" => q.OrderBy(t => t.Price),
                "pricedesc" => q.OrderByDescending(t => t.Price),
                _ => q.OrderBy(t => t.Id)
            };

            var total = q.Count();

            var items = q.Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

            var mapped = items.Select(t =>
            {
                var dto = _mapper.Map<TourDto>(t);
                if (dto.Points != null) dto.Points = dto.Points.OrderBy(p => p.Order).ToList();
                return dto;
            }).ToList();

            return new PagedResult<TourDto>(mapped, total);
        }

        public IEnumerable<string> GetAllTags()
        {
            return _tourRepository.GetAllTags();
        }

       

        public PagedResult<PopularTourDto> GetPopular(int authorId, int page, int pageSize, double? lat, double? lon, double? radiusKm)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

         
            var publishedArchived = _tourRepository.GetPublishedAndArchived().ToList();
            var myDrafts = _tourRepository.GetByAuthor(authorId)
                .Where(t => t.Status == TourStatus.Draft)
                .ToList();

            var visibleTours = publishedArchived
                .Concat(myDrafts)
                .GroupBy(t => t.Id)
                .Select(g => g.First())
                .ToList();

           
            Dictionary<long, double>? distanceByTourId = null;

            var useRadius = radiusKm.HasValue && radiusKm.Value > 0 && lat.HasValue && lon.HasValue;
            if (useRadius)
            {
                distanceByTourId = new Dictionary<long, double>();

                var reqLat = lat!.Value;
                var reqLon = lon!.Value;
                var rad = radiusKm!.Value;

                double dLat = rad / 111.0;
                double dLon = rad / (Math.Cos(reqLat * Math.PI / 180.0) * 111.0);

                visibleTours = visibleTours.Where(t =>
                {
                    var candidates = t.Points.Where(p =>
                        p.Latitude >= reqLat - dLat && p.Latitude <= reqLat + dLat &&
                        p.Longitude >= reqLon - dLon && p.Longitude <= reqLon + dLon
                    );

                    var nearest = candidates
                        .Select(p => new { p, dist = HaversineKm(reqLat, reqLon, p.Latitude, p.Longitude) })
                        .Where(x => x.dist <= rad + 1e-6)
                        .OrderBy(x => x.dist)
                        .FirstOrDefault();

                    if (nearest == null) return false;

                    distanceByTourId[t.Id] = nearest.dist;
                    return true;
                }).ToList();
            }

            
            var projected = visibleTours.Select(t =>
            {
                var reviews = _tourReviewRepository.GetByTour((int)t.Id).ToList();
                var count = reviews.Count;
                var avg = count == 0 ? 0.0 : reviews.Average(r => (double)r.Rating);

                return new PopularTourDto
                {
                    TourId = (int)t.Id,
                    Name = t.Name,
                    Status = (TourDtoStatus)t.Status,
                    Popularity = Math.Round(avg, 3),
                    RatingsCount = count,
                    DistanceKm = distanceByTourId != null && distanceByTourId.TryGetValue(t.Id, out var d)
                        ? Math.Round(d, 3)
                        : null
                };
            });

            var ordered = projected
                .OrderByDescending(x => x.Popularity)
                .ThenByDescending(x => x.RatingsCount)
                .ThenBy(x => x.TourId)
                .ToList();

            var total = ordered.Count;
            var items = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<PopularTourDto>(items, total);
        }

        private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0;
            double dLat = (lat2 - lat1) * Math.PI / 180.0;
            double dLon = (lon2 - lon1) * Math.PI / 180.0;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }


        public PagedResult<AuthorTourDashboardItemDto> GetDashboard(int authorId, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var publishedArchived = _tourRepository.GetPublishedAndArchived().ToList();
            var myDrafts = _tourRepository.GetByAuthor(authorId)
                .Where(t => t.Status == TourStatus.Draft)
                .ToList();

            var visibleTours = publishedArchived
                .Concat(myDrafts)
                .GroupBy(t => t.Id)
                .Select(g => g.First())
                .OrderBy(t => t.Id)
                .ToList();

            var ids = visibleTours.Select(t => (int)t.Id).ToList();

            var reviewStats = _tourReviewRepository.GetStatsForTours(ids);
            var execStats = _tourExecutionRepository.GetStatsForTours(ids);

            var projected = visibleTours.Select(t =>
            {
                reviewStats.TryGetValue((int)t.Id, out var rs);
                execStats.TryGetValue((int)t.Id, out var es);

                return new AuthorTourDashboardItemDto
                {
                    TourId = (int)t.Id,
                    Name = t.Name,
                    Status = (TourDtoStatus)t.Status,
                    Price = t.Price,
                    LengthInKm = t.LengthInKm,
                    PublishedAt = t.PublishedAt,
                    ArchivedAt = t.ArchivedAt,

                    Popularity = Math.Round(rs?.AvgRating ?? 0.0, 3),
                    RatingsCount = rs?.Count ?? 0,

                    Starts = es?.Starts ?? 0,
                    Completed = es?.Completed ?? 0,
                    Abandoned = es?.Abandoned ?? 0,
                    Active = es?.Active ?? 0
                };
            })
            .OrderByDescending(x => x.Popularity)
            .ThenByDescending(x => x.RatingsCount)
            .ThenBy(x => x.TourId)
            .ToList();

            var total = projected.Count;
            var items = projected.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<AuthorTourDashboardItemDto>(items, total);
        }

        public AuthorTourDashboardDetailsDto GetDashboardDetails(int authorId, int tourId, int days = 30)
        {
            if (days <= 0) days = 30;

            var tour = _tourRepository.GetById(tourId)
                ?? throw new KeyNotFoundException($"Tour {tourId} not found.");

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("Not your tour.");

            var tourDto = _mapper.Map<TourDto>(tour);
            if (tourDto.Points != null)
                tourDto.Points = tourDto.Points.OrderBy(p => p.Order).ToList();

            // review aggregate
            var reviewStats = _tourReviewRepository.GetStatsForTours(new[] { tourId });
            reviewStats.TryGetValue(tourId, out var rs);

            // execution aggregate
            var execStats = _tourExecutionRepository.GetStatsForTours(new[] { tourId });
            execStats.TryGetValue(tourId, out var es);

            // time range for trends
            var to = DateTime.UtcNow;
            var from = to.Date.AddDays(-days + 1);

            // execution trends (zahteva repo metode koje dodajemo u nastavku)
            var startsTrend = _tourExecutionRepository.GetDailyStarts(tourId, from, to);
            var completedTrend = _tourExecutionRepository.GetDailyCompleted(tourId, from, to);
            var abandonedTrend = _tourExecutionRepository.GetDailyAbandoned(tourId, from, to);

            // latest reviews
            var latestReviews = _tourReviewRepository.GetLatestForTour(tourId, take: 5);

            return new AuthorTourDashboardDetailsDto
            {
                Tour = tourDto,

                Popularity = Math.Round(rs?.AvgRating ?? 0.0, 3),
                RatingsCount = rs?.Count ?? 0,

                Starts = es?.Starts ?? 0,
                Completed = es?.Completed ?? 0,
                Abandoned = es?.Abandoned ?? 0,
                Active = es?.Active ?? 0,

                StartsTrend = FillDaysWithZeros(from, to, startsTrend),
                CompletedTrend = FillDaysWithZeros(from, to, completedTrend),
                AbandonedTrend = FillDaysWithZeros(from, to, abandonedTrend),

                LatestReviews = latestReviews.Select(_mapper.Map<TourReviewDTO>).ToList()
            };
        }

        private static List<TrendPointDto> FillDaysWithZeros(
    DateTime from, DateTime to,
    List<(DateTime Date, int Count)> data)
        {
            var dict = data.ToDictionary(x => x.Date.Date, x => x.Count);

            var result = new List<TrendPointDto>();
            for (var d = from.Date; d <= to.Date; d = d.AddDays(1))
            {
                result.Add(new TrendPointDto
                {
                    Date = d,
                    Count = dict.TryGetValue(d, out var c) ? c : 0
                });
            }
            return result;
        }
    }
}

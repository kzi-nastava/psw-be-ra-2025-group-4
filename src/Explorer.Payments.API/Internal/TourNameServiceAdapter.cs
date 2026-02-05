using System;
using Explorer.Payments.API.Internal;

namespace Explorer.Payments.Infrastructure.Internal
{
    public class TourNameServiceAdapter : ITourNameService
    {
        private readonly object _tourInfoService;

        public TourNameServiceAdapter(object tourInfoService)
        {
            _tourInfoService = tourInfoService ?? throw new ArgumentNullException(nameof(tourInfoService));
        }

        public string? GetTourName(int tourId)
        {
            try
            {
                var mi = _tourInfoService.GetType().GetMethod("Get", new[] { typeof(int) });
                if (mi == null) return null;

                var dto = mi.Invoke(_tourInfoService, new object[] { tourId });
                if (dto == null) return null;

                var nameProp = dto.GetType().GetProperty("Name");
                return nameProp?.GetValue(dto)?.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}

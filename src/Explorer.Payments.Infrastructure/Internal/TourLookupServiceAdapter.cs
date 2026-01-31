using System;
using Explorer.Payments.API.Internal;

namespace Explorer.Payments.Infrastructure.Internal
{
    public class TourLookupServiceAdapter : ITourLookupService
    {
        private readonly object _tourInfoService;
        private readonly Type _tourInfoServiceType;

        public TourLookupServiceAdapter(object tourInfoService)
        {
            _tourInfoService = tourInfoService ?? throw new ArgumentNullException(nameof(tourInfoService));
            _tourInfoServiceType = tourInfoService.GetType();
        }

        public TourLookupDto? Get(int tourId)
        {
            try
            {
                var method = _tourInfoServiceType.GetMethod("Get", new[] { typeof(int) });
                if (method == null) return null;

                var tourObj = method.Invoke(_tourInfoService, new object[] { tourId });
                if (tourObj == null) return null;

                var tourType = tourObj.GetType();

                var nameProp = tourType.GetProperty("Name");
                var authorProp = tourType.GetProperty("AuthorId");
                var tourIdProp = tourType.GetProperty("TourId") ?? tourType.GetProperty("Id");

                var name = nameProp?.GetValue(tourObj) as string;
                var authorId = authorProp?.GetValue(tourObj);
                var tid = tourIdProp?.GetValue(tourObj);

                if (authorId == null) return null;

                return new TourLookupDto
                {
                    TourId = tid is int i ? i : tourId,
                    Name = name,
                    AuthorId = Convert.ToInt32(authorId)
                };
            }
            catch
            {
                return null;
            }
        }
    }

    public class NullTourLookupService : ITourLookupService
    {
        public TourLookupDto? Get(int tourId) => null;
    }
}

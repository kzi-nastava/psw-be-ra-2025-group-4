using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IRatingService
    {
        RatingDto CreateRating(RatingCreateDto dto);
        RatingDto UpdateRating(long id, long userId, RatingUpdateDto dto);
        void DeleteRating(long ratingId, long userId);
        List<RatingDto> GetAll();
        RatingDto GetById(long ratingId);
        List<RatingDto> GetByUser(long userId);

    }
}

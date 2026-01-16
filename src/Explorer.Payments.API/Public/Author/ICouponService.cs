using Explorer.Payments.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Payments.API.Public
{
    public interface ICouponService
    {
        CouponResponseDto Create(CouponCreateDto couponDto, int authorId);
        CouponResponseDto Update(CouponUpdateDto couponDto, int authorId);
        void Delete(int id, int authorId);
        CouponResponseDto GetById(int id);
        List<CouponResponseDto> GetByAuthor(int authorId);
        CouponValidationResultDto ValidateCoupon(CouponValidationDto validationDto, Dictionary<int, PaymentTourInfoDto> tourInfos);
        CouponResponseDto ApplyCoupon(string code, int touristId, List<OrderItemDto> cartItemDtos, Dictionary<int, int> tourAuthors);
    }
}
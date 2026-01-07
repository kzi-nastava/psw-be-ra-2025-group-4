using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Payments.Core.UseCases
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;

        public CouponService(
            ICouponRepository couponRepository,
            IMapper mapper)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
        }

        public CouponResponseDto Create(CouponCreateDto couponDto, int authorId)
        {
            // Generisanje jedinstvenog koda
            string code;
            do
            {
                code = Coupon.GenerateCode();
            } while (_couponRepository.CodeExists(code));

            // ✅ NEMA validacije ture ovde - to radi controller

            var coupon = new Coupon(
                code,
                couponDto.DiscountPercentage,
                authorId,
                couponDto.ExpirationDate,
                couponDto.TourId
            );

            var created = _couponRepository.Create(coupon);
            return MapToResponseDto(created);
        }

        public CouponResponseDto Update(CouponUpdateDto couponDto, int authorId)
        {
            var coupon = _couponRepository.GetById(couponDto.Id);

            if (coupon.AuthorId != authorId)
                throw new UnauthorizedAccessException("You can only update your own coupons.");

            // ✅ NEMA validacije ture ovde - to radi controller

            coupon.Update(
                couponDto.DiscountPercentage,
                couponDto.ExpirationDate,
                couponDto.TourId
            );

            var updated = _couponRepository.Update(coupon);
            return MapToResponseDto(updated);
        }

        public void Delete(int id, int authorId)
        {
            var coupon = _couponRepository.GetById(id);

            if (coupon.AuthorId != authorId)
                throw new UnauthorizedAccessException("You can only delete your own coupons.");

            if (coupon.IsUsed)
                throw new InvalidOperationException("Cannot delete a used coupon.");

            _couponRepository.Delete(id);
        }

        public CouponResponseDto GetById(int id)
        {
            var coupon = _couponRepository.GetById(id);
            return MapToResponseDto(coupon);
        }

        public List<CouponResponseDto> GetByAuthor(int authorId)
        {
            var coupons = _couponRepository.GetByAuthor(authorId);
            return coupons.Select(MapToResponseDto).ToList();
        }

        // ✅ NOVA verzija - prima dictionary sa tour podacima
        public CouponValidationResultDto ValidateCoupon(CouponValidationDto validationDto, Dictionary<int, PaymentTourInfoDto> tourInfos)
        {
            try
            {
                var coupon = _couponRepository.GetByCode(validationDto.Code);

                if (!coupon.IsValid())
                {
                    return new CouponValidationResultDto
                    {
                        IsValid = false,
                        Message = coupon.IsUsed ? "Coupon has already been used." : "Coupon has expired."
                    };
                }

                // Pronalaženje tura od autora kupona u korpi
                var authorTours = tourInfos
                    .Where(kv => validationDto.TourIds.Contains(kv.Key) && kv.Value.AuthorId == coupon.AuthorId)
                    .ToList();

                if (!authorTours.Any())
                {
                    return new CouponValidationResultDto
                    {
                        IsValid = false,
                        Message = "This coupon is not applicable to any tours in your cart."
                    };
                }

                // Određivanje na koju turu se odnosi kupon
                KeyValuePair<int, PaymentTourInfoDto> applicableTour;
                if (coupon.TourId.HasValue)
                {
                    // Kupon za specifičnu turu
                    applicableTour = authorTours.FirstOrDefault(t => t.Key == coupon.TourId.Value);
                    if (applicableTour.Key == 0)
                    {
                        return new CouponValidationResultDto
                        {
                            IsValid = false,
                            Message = "This coupon is not applicable to any tours in your cart."
                        };
                    }
                }
                else
                {
                    // Kupon važi za sve ture autora - primenjuje se na najskuplju
                    applicableTour = authorTours.OrderByDescending(t => t.Value.Price).First();
                }

                var discountAmount = applicableTour.Value.Price * coupon.DiscountPercentage / 100;

                return new CouponValidationResultDto
                {
                    IsValid = true,
                    Message = $"Coupon valid! {coupon.DiscountPercentage}% discount on {applicableTour.Value.Name}",
                    ApplicableTourId = applicableTour.Key,
                    DiscountAmount = discountAmount,
                    DiscountPercentage = coupon.DiscountPercentage
                };
            }
            catch (NotFoundException)
            {
                return new CouponValidationResultDto
                {
                    IsValid = false,
                    Message = "Invalid coupon code."
                };
            }
        }

        // ✅ NOVA verzija - prima tour info
        public CouponResponseDto ApplyCoupon(string code, int touristId, List<OrderItemDto> cartItemDtos, Dictionary<int, int> tourAuthors)
        {
            var coupon = _couponRepository.GetByCode(code);

            if (!coupon.IsValid())
                throw new InvalidOperationException(coupon.IsUsed ? "Coupon has already been used." : "Coupon has expired.");

            // Provera da li korpa sadrži ture od autora kupona
            var authorTourIds = cartItemDtos
                .Where(item => tourAuthors.ContainsKey(item.TourId) && tourAuthors[item.TourId] == coupon.AuthorId)
                .Select(item => item.TourId)
                .ToList();

            if (!authorTourIds.Any())
                throw new InvalidOperationException("This coupon is not applicable to any tours in your cart.");

            // Provera da li je kupon za specifičnu turu
            if (coupon.TourId.HasValue && !authorTourIds.Contains(coupon.TourId.Value))
                throw new InvalidOperationException("This coupon is not applicable to any tours in your cart.");

            coupon.MarkAsUsed(touristId);
            var updated = _couponRepository.Update(coupon);
            return MapToResponseDto(updated);
        }

        private CouponResponseDto MapToResponseDto(Coupon coupon)
        {
            var dto = _mapper.Map<CouponResponseDto>(coupon);
            dto.IsValid = coupon.IsValid();

            if (coupon.TourId.HasValue)
            {
                dto.TourName = $"Tour #{coupon.TourId}"; // ✅ Samo ID, bez pozivanja servisa
            }
            else
            {
                dto.TourName = "All author's tours";
            }

            return dto;
        }
    }
}
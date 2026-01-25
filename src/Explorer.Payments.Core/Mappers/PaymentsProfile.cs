using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Dtos.Explorer.Payments.API.Dtos;
using Explorer.Payments.Core.Domain;


namespace Explorer.Payments.Core.Mappers;

public class PaymentsProfile : Profile
{
    public PaymentsProfile()
    {
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();

        CreateMap<ShoppingCart, ShoppingCartDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<TourPurchaseToken, TourPurchaseTokenDto>().ReverseMap();
        
        CreateMap<Wallet, WalletDto>().ReverseMap();

        CreateMap<Coupon, CouponResponseDto>();
        CreateMap<CouponCreateDto, Coupon>();
        CreateMap<CouponUpdateDto, Coupon>();
        CreateMap<AffiliateCode, AffiliateCodeDto>();
        CreateMap<TouristReferralInvite, TouristReferralInviteDto>();


        CreateMap<CoinsBundle, CoinsBundleDto>().ReverseMap();
        CreateMap<CoinsBundleSale, CoinsBundleSaleDto>().ReverseMap();
        CreateMap<CoinsBundlePurchase, CoinsBundlePurchaseDto>()
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
            .ReverseMap();

    }
}

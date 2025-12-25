using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain;


namespace Explorer.Payments.Core.Mappers;

public class PaymentsProfile : Profile
{
    public PaymentsProfile()
    {
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();

        CreateMap<ShoppingCart, ShoppingCartDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

       // CreateMap<TourPurchaseToken, TourPurchaseTokenDto>().ReverseMap();
    }
}

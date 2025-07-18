using AutoMapper;
using BookingPlatform.Application.Dtos.Discounts;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class DiscountProfile : Profile
{
    public DiscountProfile()
    {
        CreateMap<CreateDiscountDto, Discount>();
        CreateMap<UpdateDiscountDto, Discount>();
        CreateMap<Discount, DiscountResponseDto>();
    }
}


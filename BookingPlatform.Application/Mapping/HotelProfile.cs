using AutoMapper;
using BookingPlatform.Application.Dtos.Hotels;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<CreateHotelDto, Hotel>();
        CreateMap<UpdateHotelDto, Hotel>();
        CreateMap<Hotel, HotelResponseDto>();
        CreateMap<Hotel, HotelDetailsDto>();
        CreateMap<Hotel, HotelSearchDto>();
    }
}


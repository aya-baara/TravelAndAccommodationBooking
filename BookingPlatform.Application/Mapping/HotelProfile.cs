using AutoMapper;
using BookingPlatform.Application.Hotels.Commands.Create;
using BookingPlatform.Application.Hotels.Dtos;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<CreateHotelCommand, Hotel>();
        CreateMap<Hotel, HotelResponseDto>();
    }
}


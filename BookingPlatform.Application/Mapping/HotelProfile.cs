using AutoMapper;
using BookingPlatform.Application.Hotels.Commands.Create;
using BookingPlatform.Application.Hotels.Commands.Update;
using BookingPlatform.Application.Hotels.Dtos;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<CreateHotelCommand, Hotel>();
           
        CreateMap<UpdateHotelCommand,Hotel>()
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Hotel, HotelResponseDto>();
        CreateMap<Hotel, HotelDetailsDto>();
    }
}


using AutoMapper;
using BookingPlatform.Application.Dtos.Cities;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<City, CityResponseDto>();
        CreateMap<CreateCityDto, City>();
        CreateMap<UpdateCityDto, City>();
    }
}


using AutoMapper;
using BookingPlatform.Application.Dtos.Owners;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class OwnerProfile : Profile
{
    public OwnerProfile()
    {
        CreateMap<Owner, OwnerResponseDto>();
        CreateMap<CreateOwnerDto, Owner>();
        CreateMap<UpdateOwnerDto, Owner>();
    }
}


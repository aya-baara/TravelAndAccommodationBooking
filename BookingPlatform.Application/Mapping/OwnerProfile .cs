using AutoMapper;
using BookingPlatform.Application.Owners.Commands.Create;
using BookingPlatform.Application.Owners.Commands.Update;
using BookingPlatform.Application.Owners.Dtos;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class OwnerProfile : Profile
{
    public OwnerProfile()
    {
        CreateMap<Owner, OwnerResponseDto>();
        CreateMap<CreateOwnerCommand, Owner>();
        CreateMap<UpdateOwnerCommand, Owner>();
    }
}


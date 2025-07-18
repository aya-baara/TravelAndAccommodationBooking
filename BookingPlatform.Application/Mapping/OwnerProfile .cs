using AutoMapper;
using BookingPlatform.Application.Dtos.Owners;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class OwnerProfile : Profile
{
    public OwnerProfile()
    {
        Console.WriteLine("OwnerProfile loaded"); // Just for testing

        CreateMap<DateOnly, DateTime>().ConvertUsing(d => d.ToDateTime(new TimeOnly(0, 0)));
        CreateMap<DateTime, DateOnly>().ConvertUsing(d => DateOnly.FromDateTime(d));
        CreateMap<Owner, OwnerResponseDto>();
        CreateMap<CreateOwnerDto, Owner>();
        CreateMap<UpdateOwnerDto, Owner>();
    }
}


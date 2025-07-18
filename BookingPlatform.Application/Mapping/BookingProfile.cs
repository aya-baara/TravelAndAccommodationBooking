using AutoMapper;
using BookingPlatform.Application.Dtos.Bookings;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<CreateBookingDto, Booking>();
        CreateMap<UpdateBookingDto, Booking>();
        CreateMap<Booking, BookingResponseDto>();
    }
}


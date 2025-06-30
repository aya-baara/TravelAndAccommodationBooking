using AutoMapper;
using BookingPlatform.Application.Dtos.Reviews;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, ReviewResponseDto>();
    }
}


using AutoMapper;
using BookingPlatform.Application.Dtos.Reviews;
using BookingPlatform.WebAPI.Dtos.Reviews;

namespace BookingPlatform.WebAPI.Mapping;

public class ReviewRequestProfile : Profile
{
    public ReviewRequestProfile()
    {
        CreateMap<CreateReviewRequestDto, CreateReviewDto>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.HotelId, opt => opt.Ignore());

        CreateMap<UpdateReviewRequestDto, UpdateReviewDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.HotelId, opt => opt.Ignore());
    }

}


using AutoMapper;
using BookingPlatform.Application.Dtos.Images;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class ImageProfile : Profile
{
    public ImageProfile()
    {
        CreateMap<Image, ImageResponseDto>();
        CreateMap<CreateImageDto, Image>();
        CreateMap<UpdateImageDto, Image>();
    }
}


using AutoMapper;
using BookingPlatform.Application.Images.Dtos;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class ImageProfile :Profile
{
    public ImageProfile()
    {
        CreateMap<Image, ImageResponseDto>();
    }
}


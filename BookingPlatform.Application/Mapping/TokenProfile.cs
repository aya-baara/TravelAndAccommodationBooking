using AutoMapper;
using BookingPlatform.Application.Dtos.Users;
using BookingPlatform.Core.Models;

namespace BookingPlatform.Application.Mapping;

public class TokenProfile : Profile
{
    public TokenProfile()
    {
        CreateMap<AuthToken, TokenResponseDto>();
    }

}


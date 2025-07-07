using AutoMapper;
using BookingPlatform.Application.Dtos.Users;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class UserProfile :Profile
{
    public UserProfile()
    {
        CreateMap<SignUpDto, User>();
    }
}


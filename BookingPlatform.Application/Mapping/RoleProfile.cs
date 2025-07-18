using AutoMapper;
using BookingPlatform.Application.Dtos.Roles;
using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Mapping;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<CreateRoleDto, Role>();
        CreateMap<Role, RoleResponseDto>();
    }
}


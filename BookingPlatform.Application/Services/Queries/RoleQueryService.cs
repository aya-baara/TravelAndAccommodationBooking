using AutoMapper;
using BookingPlatform.Application.Dtos.Roles;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Queries;

public class RoleQueryService :IRoleQueryService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleQueryService> _logger;

    public RoleQueryService(IRoleRepository roleRepository
        , IMapper mapper
        , ILogger<RoleQueryService> logger)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<RoleResponseDto> GetRoleByIdAsync(Guid id, CancellationToken ct)
    {
        var role = await _roleRepository.GetRoleByIdAsync(id, ct);
        if (role is null)
        {
            _logger.LogWarning($"Role with ID {id} not found");
            throw new NotFoundException("The Requested Role Not found");
        }

        _logger.LogInformation($"Successfully retrieved Role with ID {id}");

        return _mapper.Map<RoleResponseDto>(role);
    }
}


using AutoMapper;
using BookingPlatform.Application.Dtos.Roles;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Commands;

public class RoleCommandService : IRoleCommandService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RoleCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public RoleCommandService(IRoleRepository roleRepository
        , IMapper mapper
        , ILogger<RoleCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
       _logger = logger;
       _unitOfWork = unitOfWork;
    }

    public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto dto, CancellationToken ct)
    {
        var role = _mapper.Map<Role>(dto);
        var created = await _roleRepository.CreateRoleAsync(role, ct);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Role Created successfully with ID {created.Id}");

        return _mapper.Map<RoleResponseDto>(created);
    }

    public async Task DeleteRoleAsync(Guid id, CancellationToken ct)
    {
        var role = await _roleRepository.GetRoleByIdAsync(id, ct);
        if (role is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Role {id}");
            throw new NotFoundException("The Requested Role Not found");
        }
        await _roleRepository.DeleteRoleAsync(id, ct);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Role Deleted successfully with ID {id}");
    }
}


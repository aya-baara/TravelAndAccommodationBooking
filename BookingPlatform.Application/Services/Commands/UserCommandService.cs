using AutoMapper;
using BookingPlatform.Application.Dtos.Users;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Commands;

public class UserCommandService : IUserCommandService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    private readonly ILogger<UserCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UserCommandService(IUserRepository userRepository
        , IRoleRepository roleRepository
        , IPasswordHasher passwordHasher
        , IMapper mapper
        , ILogger<UserCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task SignUpAsync(CreateUserDto dto, CancellationToken ct)
    {
        _logger.LogInformation("User signup attempt for email: {Email}", dto.Email);

        var role = await _roleRepository.GetRoleByIdAsync(dto.RoleId, ct);
        if (role is null)
        {
            _logger.LogWarning("Role with ID {RoleId} not found during signup.", dto.RoleId);
            throw new NotFoundException("The requested role was not found.");
        }

        if (await _userRepository.DoesUserExistAsync(dto.Email, ct))
        {
            _logger.LogWarning("Signup attempt with existing email: {Email}", dto.Email);
            throw new EmailAlreadyExistsException($"User with email {dto.Email} already exists.");
        }

        var userToAdd = _mapper.Map<User>(dto);
        userToAdd.Role = role;
        userToAdd.Password = _passwordHasher.HashPassword(dto.Password);

        await _userRepository.CreateUserAsync(userToAdd, ct);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User with email {Email} created successfully.", dto.Email);
    }

}

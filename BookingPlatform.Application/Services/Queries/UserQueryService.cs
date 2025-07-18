using AutoMapper;
using BookingPlatform.Application.Dtos.Users;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Auth;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Queries;

public class UserQueryService : IUserQueryService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IMapper _mapper;
    private readonly ILogger<UserQueryService> _logger;

    public UserQueryService(IUserRepository userRepository
        , ITokenGenerator tokenGenerator
        , IMapper mapper
        , ILogger<UserQueryService> looger)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _mapper = mapper;
        _logger = looger;
    }

    public async Task<TokenResponseDto> LogInAsync(LogInDto dto, CancellationToken ct)
    {
        _logger.LogInformation("User login attempt for email: {Email}", dto.Email);

        var user = await _userRepository.AuthenticateUserAsync(dto.Email, dto.Password, ct);
        if (user is null)
        {
            _logger.LogWarning($"Attempted to Login with Invalid email or password");

            throw new CredentialsNotValidException("Invalid email or password.");
        }

        _logger.LogInformation("User login successful for email: {Email}", dto.Email);

        return _mapper.Map<TokenResponseDto>(_tokenGenerator.GenerateToken(user));
    }

}


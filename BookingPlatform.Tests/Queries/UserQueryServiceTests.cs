using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using BookingPlatform.Application.Dtos.Users;
using BookingPlatform.Application.Services.Queries;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Auth;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Queries;

public class UserQueryServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRoleRepository> _roleRepoMock;
    private readonly Mock<ITokenGenerator> _tokenGenMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UserQueryService>> _loggerMock;
    private readonly UserQueryService _service;

    public UserQueryServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        // Avoid recursion issues
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _userRepoMock = _fixture.Freeze<Mock<IUserRepository>>();
        _roleRepoMock = _fixture.Freeze<Mock<IRoleRepository>>();
        _tokenGenMock = _fixture.Freeze<Mock<ITokenGenerator>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();
        _loggerMock = _fixture.Freeze<Mock<ILogger<UserQueryService>>>();

        _service = new UserQueryService(
            _userRepoMock.Object,
            _roleRepoMock.Object,
            _tokenGenMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task LogInAsync_ShouldReturnTokenResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var dto = _fixture.Create<LogInDto>();
        var user = _fixture.Build<User>()
            .Without(u => u.Role) // We'll assign it explicitly
            .Create();
        var role = _fixture.Create<Role>();
        var token = _fixture.Create<AuthToken>();
        var expectedTokenDto = _fixture.Create<TokenResponseDto>();

        _userRepoMock.Setup(x => x.AuthenticateUserAsync(dto.Email, dto.Password, default))
            .ReturnsAsync(user);
        _roleRepoMock.Setup(x => x.GetRoleByIdAsync(user.RoleId, default))
            .ReturnsAsync(role);
        _tokenGenMock.Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns(token);
        _mapperMock.Setup(x => x.Map<TokenResponseDto>(token))
            .Returns(expectedTokenDto);

        // Act
        var result = await _service.LogInAsync(dto, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTokenDto, result);
        _userRepoMock.Verify(x => x.AuthenticateUserAsync(dto.Email, dto.Password, default), Times.Once);
        _roleRepoMock.Verify(x => x.GetRoleByIdAsync(user.RoleId, default), Times.Once);
        _tokenGenMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Once);
        _mapperMock.Verify(x => x.Map<TokenResponseDto>(token), Times.Once);
    }

    [Fact]
    public async Task LogInAsync_ShouldThrowException_WhenUserIsNull()
    {
        // Arrange
        var dto = _fixture.Create<LogInDto>();

        _userRepoMock.Setup(x => x.AuthenticateUserAsync(dto.Email, dto.Password, default))
            .ReturnsAsync((User)null);

        // Act & Assert
        await Assert.ThrowsAsync<CredentialsNotValidException>(() => _service.LogInAsync(dto, default));

        _userRepoMock.Verify(x => x.AuthenticateUserAsync(dto.Email, dto.Password, default), Times.Once);
    }
}

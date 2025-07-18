using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using BookingPlatform.Application.Dtos.Users;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Commands;

public class UserCommandServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRoleRepository> _roleRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UserCommandService>> _loggerMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UserCommandService _sut;

    public UserCommandServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _fixture.Customize<DateOnly>(c =>
            c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20)))
        );
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _userRepoMock = _fixture.Freeze<Mock<IUserRepository>>();
        _roleRepoMock = _fixture.Freeze<Mock<IRoleRepository>>();
        _passwordHasherMock = _fixture.Freeze<Mock<IPasswordHasher>>();
        _mapperMock = _fixture.Freeze<Mock<IMapper>>();
        _loggerMock = _fixture.Freeze<Mock<ILogger<UserCommandService>>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();

        _sut = new UserCommandService(
            _userRepoMock.Object,
            _roleRepoMock.Object,
            _passwordHasherMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object
        );
    }


    [Fact]
    public async Task SignUpAsync_ShouldCreateUser_WhenDataIsValid()
    {
        // Arrange
        var dto = _fixture.Create<CreateUserDto>();
        var role = _fixture.Create<Role>();
        var user = _fixture.Create<User>();

        _roleRepoMock.Setup(r => r.GetRoleByIdAsync(dto.RoleId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(role);

        _userRepoMock.Setup(u => u.DoesUserExistAsync(dto.Email, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

        _mapperMock.Setup(m => m.Map<User>(dto))
                   .Returns(user);

        _passwordHasherMock.Setup(p => p.HashPassword(dto.Password))
                           .Returns("hashed-password");

        // Act
        await _sut.SignUpAsync(dto, CancellationToken.None);

        // Assert
        _userRepoMock.Verify(u => u.CreateUserAsync(It.Is<User>(u =>
            u.Password == "hashed-password" &&
            u.Role == role
        ), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SignUpAsync_ShouldThrowNotFoundException_WhenRoleNotFound()
    {
        // Arrange
        var dto = _fixture.Create<CreateUserDto>();
        _roleRepoMock.Setup(r => r.GetRoleByIdAsync(dto.RoleId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Role?)null);

        // Act
        var act = async () => await _sut.SignUpAsync(dto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
                 .WithMessage("The requested role was not found.");
    }

    [Fact]
    public async Task SignUpAsync_ShouldThrowEmailAlreadyExistsException_WhenEmailExists()
    {
        // Arrange
        var dto = _fixture.Create<CreateUserDto>();
        var role = _fixture.Create<Role>();

        _roleRepoMock.Setup(r => r.GetRoleByIdAsync(dto.RoleId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(role);

        _userRepoMock.Setup(u => u.DoesUserExistAsync(dto.Email, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);

        // Act
        var act = async () => await _sut.SignUpAsync(dto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EmailAlreadyExistsException>()
                 .WithMessage($"User with email {dto.Email} already exists.");
    }
}

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using BookingPlatform.Application.Dtos.Owners;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Commands;

public class OwnerCommandServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IOwnerRepository> _ownerRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<OwnerCommandService>> _loggerMock;
    private readonly OwnerCommandService _sut;

    public OwnerCommandServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<DateOnly>(c =>
         c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20)))
        );
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
        .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _ownerRepoMock = new Mock<IOwnerRepository>();
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<OwnerCommandService>>();

        _sut = new OwnerCommandService(
            _ownerRepoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateOwnerAsync_ShouldReturnDto_WhenValid()
    {
        // Arrange
        var dto = _fixture.Create<CreateOwnerDto>();
        var owner = _fixture.Create<Owner>();
        var createdOwner = _fixture.Create<Owner>();
        var resultDto = _fixture.Create<OwnerResponseDto>();

        _mapperMock.Setup(m => m.Map<Owner>(dto)).Returns(owner);
        _ownerRepoMock.Setup(r => r.CreateOwnerAsync(owner, default)).ReturnsAsync(createdOwner);
        _mapperMock.Setup(m => m.Map<OwnerResponseDto>(createdOwner)).Returns(resultDto);

        // Act
        var result = await _sut.CreateOwnerAsync(dto, default);

        // Assert
        result.Should().BeEquivalentTo(resultDto);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteOwnerAsync_ShouldSucceed_WhenOwnerExists()
    {
        var owner = _fixture.Create<Owner>();

        _ownerRepoMock.Setup(r => r.GetOwnerByIdAsync(owner.Id, default)).ReturnsAsync(owner);

        await _sut.DeleteOwnerAsync(owner.Id, default);

        _ownerRepoMock.Verify(r => r.DeleteOwnerById(owner.Id, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteOwnerAsync_ShouldThrow_WhenOwnerNotFound()
    {
        var id = Guid.NewGuid();
        _ownerRepoMock.Setup(r => r.GetOwnerByIdAsync(id, default)).ReturnsAsync((Owner)null!);

        Func<Task> act = async () => await _sut.DeleteOwnerAsync(id, default);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Owner Not found");
    }

    [Fact]
    public async Task UpdateOwnerAsync_ShouldSucceed_WhenOwnerExists()
    {
        var dto = _fixture.Create<UpdateOwnerDto>();
        var owner = _fixture.Build<Owner>().With(o => o.Id, dto.Id).Create();

        _ownerRepoMock.Setup(r => r.GetOwnerByIdAsync(dto.Id, default)).ReturnsAsync(owner);

        await _sut.UpdateOwnerAsync(dto, default);

        _mapperMock.Verify(m => m.Map(dto, owner), Times.Once);
        _ownerRepoMock.Verify(r => r.UpdateOwner(owner, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateOwnerAsync_ShouldThrow_WhenOwnerNotFound()
    {
        var dto = _fixture.Create<UpdateOwnerDto>();
        _ownerRepoMock.Setup(r => r.GetOwnerByIdAsync(dto.Id, default)).ReturnsAsync((Owner)null!);

        Func<Task> act = async () => await _sut.UpdateOwnerAsync(dto, default);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Owner Not found");
    }
}

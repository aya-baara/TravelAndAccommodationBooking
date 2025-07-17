using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using BookingPlatform.Application.Dtos.Reviews;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Commands;

public class ReviewCommandServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IReviewRepository> _reviewRepoMock;
    private readonly Mock<IHotelRepository> _hotelRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<ReviewCommandService>> _loggerMock;
    private readonly ReviewCommandService _sut;

    public ReviewCommandServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Customize<DateOnly>(c =>
         c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20)))
        );
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _reviewRepoMock = new Mock<IReviewRepository>();
        _hotelRepoMock = new Mock<IHotelRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<ReviewCommandService>>();

        _sut = new ReviewCommandService(
            _reviewRepoMock.Object,
            _hotelRepoMock.Object,
            _userRepoMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldReturnDto_WhenValid()
    {
        // Arrange
        var dto = _fixture.Create<CreateReviewDto>();
        var user = _fixture.Create<User>();
        var hotel = _fixture.Create<Hotel>();
        var review = _fixture.Create<Review>();
        var resultDto = _fixture.Create<ReviewResponseDto>();

        _userRepoMock.Setup(r => r.GetUserByIdAsync(dto.UserId, default)).ReturnsAsync(user);
        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(dto.HotelId, default)).ReturnsAsync(hotel);
        _mapperMock.Setup(m => m.Map<Review>(dto)).Returns(review);
        _reviewRepoMock.Setup(r => r.CreateReviewAsync(review, default)).ReturnsAsync(review);
        _mapperMock.Setup(m => m.Map<ReviewResponseDto>(review)).Returns(resultDto);

        // Act
        var result = await _sut.CreateReviewAsync(dto, default);

        // Assert
        result.Should().BeEquivalentTo(resultDto);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldThrow_WhenUserNotFound()
    {
        var dto = _fixture.Create<CreateReviewDto>();
        _userRepoMock.Setup(r => r.GetUserByIdAsync(dto.UserId, default)).ReturnsAsync((User)null!);

        Func<Task> act = async () => await _sut.CreateReviewAsync(dto, default);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested User Not found");
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldThrow_WhenHotelNotFound()
    {
        var dto = _fixture.Create<CreateReviewDto>();
        _userRepoMock.Setup(r => r.GetUserByIdAsync(dto.UserId, default)).ReturnsAsync(_fixture.Create<User>());
        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(dto.HotelId, default)).ReturnsAsync((Hotel)null!);

        Func<Task> act = async () => await _sut.CreateReviewAsync(dto, default);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Hotel Not found");
    }

    [Fact]
    public async Task DeleteReview_ShouldSucceed_WhenAuthorized()
    {
        var review = _fixture.Create<Review>();

        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(review.Id, default)).ReturnsAsync(review);

        await _sut.DeleteReview(review.Id, review.UserId, default);

        _reviewRepoMock.Verify(r => r.DeleteReviewById(review.Id, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteReview_ShouldThrow_WhenReviewNotFound()
    {
        var id = Guid.NewGuid();
        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(id, default)).ReturnsAsync((Review)null!);

        Func<Task> act = async () => await _sut.DeleteReview(id, Guid.NewGuid(), default);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Review Not found");
    }

    [Fact]
    public async Task DeleteReview_ShouldThrowForbidden_WhenNotOwner()
    {
        var review = _fixture.Create<Review>();
        var otherUserId = Guid.NewGuid();

        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(review.Id, default)).ReturnsAsync(review);

        Func<Task> act = async () => await _sut.DeleteReview(review.Id, otherUserId, default);

        await act.Should().ThrowAsync<ForbiddenAccessException>()
            .WithMessage("You are not allowed to access this Review.");
    }

    [Fact]
    public async Task UpdateReview_ShouldSucceed_WhenAuthorized()
    {
        var dto = _fixture.Create<UpdateReviewDto>();
        var review = _fixture.Create<Review>();
        var user = _fixture.Build<User>().With(u => u.Id, dto.UserId).Create();
        var hotel = _fixture.Create<Hotel>();

        review.UserId = dto.UserId;

        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(dto.Id, default)).ReturnsAsync(review);
        _userRepoMock.Setup(u => u.GetUserByIdAsync(dto.UserId, default)).ReturnsAsync(user);
        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(dto.HotelId, default)).ReturnsAsync(hotel);

        await _sut.UpdateReview(dto, default);

        _mapperMock.Verify(m => m.Map(dto, review), Times.Once);
        _reviewRepoMock.Verify(r => r.UpdateReviewAsync(review, default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateReview_ShouldThrow_WhenReviewNotFound()
    {
        var dto = _fixture.Create<UpdateReviewDto>();
        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(dto.Id, default)).ReturnsAsync((Review)null!);

        Func<Task> act = async () => await _sut.UpdateReview(dto, default);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Review Not found");
    }

    [Fact]
    public async Task UpdateReview_ShouldThrowForbidden_WhenNotOwner()
    {
        var dto = _fixture.Create<UpdateReviewDto>();
        var review = _fixture.Create<Review>();
        review.UserId = Guid.NewGuid(); // not matching dto.UserId

        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(dto.Id, default)).ReturnsAsync(review);
        _userRepoMock.Setup(u => u.GetUserByIdAsync(dto.UserId, default)).ReturnsAsync(_fixture.Create<User>());

        Func<Task> act = async () => await _sut.UpdateReview(dto, default);

        await act.Should().ThrowAsync<ForbiddenAccessException>()
            .WithMessage("You are not allowed to access this Review.");
    }

    [Fact]
    public async Task UpdateReview_ShouldThrow_WhenUserNotFound()
    {
        var dto = _fixture.Create<UpdateReviewDto>();
        var review = _fixture.Create<Review>();
        review.UserId = dto.UserId;

        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(dto.Id, default)).ReturnsAsync(review);
        _userRepoMock.Setup(u => u.GetUserByIdAsync(dto.UserId, default)).ReturnsAsync((User)null!);

        Func<Task> act = async () => await _sut.UpdateReview(dto, default);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested User Not found");
    }

    [Fact]
    public async Task UpdateReview_ShouldThrow_WhenHotelNotFound()
    {
        var dto = _fixture.Create<UpdateReviewDto>();
        var review = _fixture.Create<Review>();
        review.UserId = dto.UserId;

        _reviewRepoMock.Setup(r => r.GetReviewByIdAsync(dto.Id, default)).ReturnsAsync(review);
        _userRepoMock.Setup(u => u.GetUserByIdAsync(dto.UserId, default)).ReturnsAsync(_fixture.Create<User>());
        _hotelRepoMock.Setup(h => h.GetHotelByIdAsync(dto.HotelId, default)).ReturnsAsync((Hotel)null!);

        Func<Task> act = async () => await _sut.UpdateReview(dto, default);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("The Requested Hotel Not found");
    }
}

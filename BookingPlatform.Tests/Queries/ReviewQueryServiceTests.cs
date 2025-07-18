using AutoFixture;
using AutoMapper;
using BookingPlatform.Application.Dtos.Reviews;
using BookingPlatform.Application.Services.Queries;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookingPlatform.Tests.Queries;

public class ReviewQueryServiceTests
{
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<ReviewQueryService>> _loggerMock;
    private readonly Fixture _fixture;
    private readonly ReviewQueryService _sut;

    public ReviewQueryServiceTests()
    {
        _fixture = new Fixture();

        _fixture.Behaviors
       .OfType<ThrowingRecursionBehavior>()
       .ToList()
       .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _fixture.Customize<DateOnly>(c =>
           c.FromFactory(() => DateOnly.FromDateTime(DateTime.Today.AddYears(-_fixture.Create<int>() % 40 - 20))));

        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<ReviewQueryService>>();

        _sut = new ReviewQueryService(
            _reviewRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetHotelReviews_ShouldReturnPaginatedResult()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var page = 1;
        var size = 5;
        var cancellationToken = CancellationToken.None;

        var reviews = _fixture.CreateMany<Review>(size).ToList();
        var paginatedReviews = new PaginatedResult<Review>(reviews, reviews.Count, page, size);
        var mappedDtos = _fixture.CreateMany<ReviewResponseDto>(size).ToList();

        _reviewRepositoryMock
            .Setup(r => r.GetReviewsByHotelIdAsync(hotelId, page, size, cancellationToken))
            .ReturnsAsync(paginatedReviews);

        _mapperMock
            .Setup(m => m.Map<List<ReviewResponseDto>>(reviews))
            .Returns(mappedDtos);

        // Act
        var result = await _sut.GetHotelReviews(hotelId, cancellationToken, page, size);

        // Assert
        Assert.Equal(mappedDtos.Count, result.Items.Count());
        Assert.Equal(page, result.Pagination.Page);
        Assert.Equal(size, result.Pagination.PageSize);
        _reviewRepositoryMock.Verify(r => r.GetReviewsByHotelIdAsync(hotelId, page, size, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetReviewByIdAsync_ShouldReturnMappedDto_WhenReviewExists()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        var review = _fixture.Create<Review>();
        var mappedDto = _fixture.Create<ReviewResponseDto>();

        _reviewRepositoryMock
            .Setup(r => r.GetReviewByIdAsync(reviewId, cancellationToken))
            .ReturnsAsync(review);

        _mapperMock
            .Setup(m => m.Map<ReviewResponseDto>(review))
            .Returns(mappedDto);

        // Act
        var result = await _sut.GetReviewByIdAsync(reviewId, cancellationToken);

        // Assert
        Assert.Equal(mappedDto, result);
        _reviewRepositoryMock.Verify(r => r.GetReviewByIdAsync(reviewId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetReviewByIdAsync_ShouldThrowNotFoundException_WhenReviewDoesNotExist()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _reviewRepositoryMock
            .Setup(r => r.GetReviewByIdAsync(reviewId, cancellationToken))
            .ReturnsAsync((Review?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.GetReviewByIdAsync(reviewId, cancellationToken));

        _reviewRepositoryMock.Verify(r => r.GetReviewByIdAsync(reviewId, cancellationToken), Times.Once);
    }
}

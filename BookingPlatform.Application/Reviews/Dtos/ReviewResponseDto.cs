using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Reviews.Dtos;

public class ReviewResponseDto
{
    public User User { get; set; }
    public string Comment { get; set; }
    public double Rate { get; set; }
}


using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Dtos.Reviews;

public class ReviewResponseDto
{
    public User User { get; set; }
    public string Comment { get; set; }
    public double Rate { get; set; }
}


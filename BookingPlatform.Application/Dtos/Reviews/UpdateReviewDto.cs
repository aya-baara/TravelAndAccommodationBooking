namespace BookingPlatform.Application.Dtos.Reviews;
public class UpdateReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Comment { get; set; }
    public double Rate { get; set; }
    public Guid HotelId { get; set; }
}



namespace BookingPlatform.Core.Entities;

public class Hotel : BaseEntity
{
    public string Name { get; set; }
    public string Location { get; set; }
    public City City { get; set; }
    public Guid CityId { get; set; }
    public int StarRating { get; set; }
    public double ReviewRating { get; set; }
    public string FullDescription { get; set; }
    public string BriefDescription { get; set; }
    public int PhoneNumber { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Owner Owner { get; set; }
    public Guid OwnerId { get; set; }
    public List<Room> Rooms { get; set; } = new List<Room>();
    public List<Review> Reviews { get; set; } = new List<Review>();
    public List<Image> Images { get; set; } = new List<Image>();
    public Image? Thumbnail { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime ModifiedAt { get; set; }

}

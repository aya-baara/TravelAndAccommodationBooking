using BookingPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingPlatform.Infrastructure.Configuration;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Name).IsRequired();
        builder.Property(h => h.City).IsRequired();

        //Hotel -> city (many-to-one)
        builder.HasOne(h => h.City)
            .WithMany(c=>c.Hotels)
            .HasForeignKey(h=>h.CityId)
            .OnDelete(DeleteBehavior.Cascade);

        //Hotel -> Owner (many-to-one)
        builder.HasOne(h=>h.Owner)
            .WithMany(o=>o.Hotels)
            .HasForeignKey(h=>h.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(h => h.Thumbnail);

        // Hotel -> Reviews (one-to-many)
        builder.HasMany(h => h.Reviews)
            .WithOne(r => r.Hotel)
            .HasForeignKey(h => h.HotelId);

        //Hotel -> Rooms (one-to-many)
        builder.HasMany(h => h.Rooms)
            .WithOne(r => r.Hotel)
            .HasForeignKey(r => r.HotelId);

    }
}


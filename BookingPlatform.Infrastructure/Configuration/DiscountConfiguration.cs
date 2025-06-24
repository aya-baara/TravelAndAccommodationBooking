using BookingPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingPlatform.Infrastructure.Configuration;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.HasKey(d => d.Id);

        builder.HasOne(d => d.Room)
            .WithMany(r => r.Discounts)
            .HasForeignKey(d => d.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(d => d.Percentage)
            .HasPrecision(8, 6);

    }
}


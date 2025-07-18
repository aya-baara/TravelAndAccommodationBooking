using BookingPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingPlatform.Infrastructure.Configuration;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b=>b.Id).HasDefaultValueSql("NEWID()");

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Rooms)
            .WithMany(r => r.Bookings);

        builder.Property(b => b.TotalPriceAfterDiscount)
            .HasColumnType("decimal(18,2)");

        builder.Property(b => b.TotalPriceBeforeDiscount)
            .HasColumnType("decimal(18,2)");
    }
}


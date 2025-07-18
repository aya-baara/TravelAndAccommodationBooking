using BookingPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingPlatform.Infrastructure.Configuration;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasDefaultValueSql("NEWID()");


        builder.Property(i => i.PaymentMethod)
            .HasConversion<string>();

        builder.HasOne(i => i.Booking)
            .WithOne(b => b.Invoice)
            .HasForeignKey<Invoice>(i => i.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(i => i.TotalAmount)
            .HasPrecision(8, 6);

    }
}


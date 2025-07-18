using BookingPlatform.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from separate files
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}


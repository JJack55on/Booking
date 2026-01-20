using Booking.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.DataAccess.Context;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options)
        : base(options)
    {
    }

    public System.Data.Entity.DbSet<Room> Rooms { get; set; }
    public System.Data.Entity.DbSet<Entities.Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Entities.Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.RoomId).IsUnique();
                
            entity.HasOne(e => e.Room)
                .WithMany(e => e.Bookings)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
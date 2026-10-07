using Microsoft.EntityFrameworkCore;
using HotelBooking.Models;

namespace HotelBooking.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _config;
        public DataContextEF(IConfiguration config)
        {
            _config = config;
        }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<RoomBooking> RoomBookings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("Default"),
                               optionsBuilder => optionsBuilder.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Rooms");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Number).IsRequired();
                entity.Property(e => e.AdultsCapacity).IsRequired();
                entity.Property(e => e.Price).IsRequired();
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Bookings");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GuestFirstName).IsRequired();
                entity.Property(e => e.GuestLastName).IsRequired();
                entity.Property(e => e.CheckInDate).IsRequired();
                entity.Property(e => e.CheckOutDate).IsRequired();
                entity.Property(e => e.NumberOfAdults).IsRequired();
            });

            modelBuilder.Entity<RoomBooking>(entity =>
            {
                entity.ToTable("RoomBookings");

                entity.HasKey(rb => new { rb.RoomId, rb.BookingId });

                entity.Property(rb => rb.RoomId).IsRequired();
                entity.Property(rb => rb.BookingId).IsRequired();

                entity.HasOne(rb => rb.Room)
                    .WithMany(r => r.RoomBookings)
                    .HasForeignKey(rb => rb.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rb => rb.Booking)
                    .WithMany()
                    .HasForeignKey(rb => rb.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
       
        
        
    }
}
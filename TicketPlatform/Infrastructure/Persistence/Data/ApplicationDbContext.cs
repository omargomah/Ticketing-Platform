using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Seat> Seats => Set<Seat>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Attendee> Attendees => Set<Attendee>();
        public DbSet<Organizer> Organizers => Set<Organizer>();
        public DbSet<Preference> Preferences => Set<Preference>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}

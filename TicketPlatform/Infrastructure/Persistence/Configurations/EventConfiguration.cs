using Domain.Entities;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title)
                .HasMaxLength(Constants.Event.TitleMaxLength)
                .IsRequired();

            builder.Property(e => e.StartTime)
                .IsRequired();

            builder.Property(e => e.EndTime)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(e => e.CoverImageUrl)
                .HasMaxLength(Constants.Event.CoverImageUrlMaxLength)
                .IsRequired(false);

            builder.OwnsOne(e => e.VenueAddress, addr =>
            {
                addr.Property(a => a.VenueName)
                    .HasColumnName("VenueName")
                    .HasMaxLength(Constants.Address.VenueNameMaxLength)
                    .IsRequired();

                addr.Property(a => a.Street)
                    .HasColumnName("Street")
                    .HasMaxLength(Constants.Address.StreetMaxLength)
                    .IsRequired();

                addr.Property(a => a.City)
                    .HasColumnName("City")
                    .HasMaxLength(Constants.Address.CityMaxLength)
                    .IsRequired();
            });

            builder.HasOne(e => e.Organizer)
                .WithMany(o => o.Events)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Preferences)
                .WithMany(p => p.Events)
                .UsingEntity("EventPreferences");
        }
    }
}

using Domain.Entities;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class AttendeeConfiguration : IEntityTypeConfiguration<Attendee>
    {
        public void Configure(EntityTypeBuilder<Attendee> builder)
        {
            builder.ToTable("Attendees");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.FirstName)
                .HasMaxLength(Constants.Attendee.FirstNameMaxLength)
                .IsRequired();

            builder.Property(a => a.LastName)
                .HasMaxLength(Constants.Attendee.LastNameMaxLength)
                .IsRequired();

            builder.Property(a => a.DateOfBirth)
                .IsRequired(false);

            builder.HasMany(a => a.Preferences)
                .WithMany(p => p.Attendees)
                .UsingEntity("AttendeePreferences");
        }
    }
}

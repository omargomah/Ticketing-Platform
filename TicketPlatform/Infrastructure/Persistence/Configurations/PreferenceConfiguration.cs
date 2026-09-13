using Domain.Entities;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
    {
        public void Configure(EntityTypeBuilder<Preference> builder)
        {
            builder.ToTable("Preferences");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(Constants.Preference.NameMaxLength)
                .IsRequired();

            builder.HasIndex(p => p.Name)
                .IsUnique();
        }
    }
}

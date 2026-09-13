using Domain.Entities;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class OrganizerConfiguration : IEntityTypeConfiguration<Organizer>
    {
        public void Configure(EntityTypeBuilder<Organizer> builder)
        {
            builder.ToTable("Organizers");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                .HasMaxLength(Constants.Organizer.NameMaxLength)
                .IsRequired();

            builder.Property(o => o.WebSite)
                .HasMaxLength(Constants.Organizer.WebSiteMaxLength)
                .IsRequired(false);

            builder.OwnsOne(o => o.TaxRegistrationNumber, tax =>
            {
                tax.Property(t => t.Value)
                    .HasColumnName("TaxRegistrationNumber")
                    .HasMaxLength(Constants.TaxRegistrationNumber.RequiredLength)
                    .IsFixedLength()
                    .IsRequired();

                tax.HasIndex(t => t.Value)
                    .IsUnique();
            });

            builder.OwnsOne(o => o.BankIban, iban =>
            {
                iban.Property(b => b.Value)
                    .HasColumnName("BankIban")
                    .HasMaxLength(Constants.BankIban.RequiredLength)
                    .IsFixedLength()
                    .IsRequired();
            });

            builder.OwnsOne(o => o.TotalRevenue, rev =>
            {
                rev.Property(r => r.Amount)
                    .HasColumnName("TotalRevenue")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });
        }
    }
}

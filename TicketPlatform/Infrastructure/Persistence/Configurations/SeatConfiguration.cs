using Domain.Entities;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.ToTable("Seats");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Row)
                .IsRequired();

            builder.Property(s => s.SeatNumber)
                .IsRequired();

            builder.Property(s => s.Type)
                .HasMaxLength(Constants.Seat.TypeMaxLength)
                .IsRequired(false);

            builder.Property(s => s.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(s => s.LockExpiration)
                .IsRequired(false);

            builder.OwnsOne(s => s.Price, pr =>
            {
                pr.Property(p => p.Amount)
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            builder.HasOne(s => s.Event)
                .WithMany(e => e.Seats)
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => new { s.EventId, s.Row, s.SeatNumber })
                .IsUnique();
        }
    }
}

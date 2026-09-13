using Domain.Entities;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.BookingAt)
                .IsRequired();

            builder.Property(b => b.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(b => b.QRCode)
                .HasMaxLength(Constants.Booking.QRCodeMaxLength)
                .IsRequired(false);

            builder.Property(b => b.PaymentReferenceId)
                .HasMaxLength(Constants.Booking.PaymentReferenceIdMaxLength)
                .IsRequired(false);

            builder.OwnsOne(b => b.Price, pr =>
            {
                pr.Property(p => p.Amount)
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            builder.HasOne(b => b.Attendee)
                .WithMany(a => a.Bookings)
                .HasForeignKey(b => b.AttendeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Seat)
                .WithMany()
                .HasForeignKey(b => b.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

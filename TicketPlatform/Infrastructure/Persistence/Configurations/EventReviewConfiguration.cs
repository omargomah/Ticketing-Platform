using Domain.Entities;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class EventReviewConfiguration : IEntityTypeConfiguration<EventReview>
    {
        public void Configure(EntityTypeBuilder<EventReview> builder)
        {
            builder.ToTable("EventReviews");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Comment)
                .HasMaxLength(Constants.EventReview.CommentMaxLength)
                .IsRequired(false);

            builder.OwnsOne(r => r.Rating, rt =>
            {
                rt.Property(p => p.Value)
                    .HasColumnName("Rating")
                    .IsRequired();
            });

            builder.HasOne(r => r.Event)
                .WithMany(e => e.Reviews)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Attendee)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.AttendeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => new { r.EventId, r.AttendeeId })
                .IsUnique();
        }
    }
}

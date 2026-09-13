using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class EventReview : Entity
    {
        public Guid EventId { get; private set; }
        public Event Event { get; private set; } = null!;

        public Guid AttendeeId { get; private set; }
        public Attendee Attendee { get; private set; } = null!;

        public Rating Rating { get; private set; }
        public string? Comment { get; private set; }

        private EventReview(
            Guid id,
            Guid eventId,
            Guid attendeeId,
            Rating rating,
            string? comment) : base(id)
        {
            EventId = eventId;
            AttendeeId = attendeeId;
            Rating = rating;
            Comment = comment;
        }

        private EventReview() : base(Guid.Empty)
        {
            Rating = null!;
        }

        public static Result<EventReview> Create(
            Guid eventId,
            Guid attendeeId,
            Rating rating,
            string? comment = null,
            Guid? id = null)
        {
            if (eventId == Guid.Empty)
                return Result.Failure<EventReview>(Error.Create("EventReview.EventIdRequired", "Event ID is required."));

            if (attendeeId == Guid.Empty)
                return Result.Failure<EventReview>(Error.Create("EventReview.AttendeeIdRequired", "Attendee ID is required."));

            if (!string.IsNullOrWhiteSpace(comment) && comment.Length > Constants.EventReview.CommentMaxLength)
                return Result.Failure<EventReview>(Error.Create("EventReview.CommentTooLong", $"Comment cannot exceed {Constants.EventReview.CommentMaxLength} characters."));

            return new EventReview(
                id ?? Guid.NewGuid(),
                eventId,
                attendeeId,
                rating,
                comment?.Trim());
        }
    }
}

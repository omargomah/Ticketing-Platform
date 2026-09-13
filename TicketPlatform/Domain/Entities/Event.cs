using Domain.Enums;
using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Event : Entity, IAggregateRoot
    {
        private readonly List<Seat> _seats = new();
        private readonly List<EventReview> _reviews = new();
        private readonly List<Preference> _preferences = new();

        public Guid OrganizerId { get; private set; }
        public Organizer Organizer { get; private set; } = null!;

        public string Title { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public EventStatus Status { get; private set; }
        public Address VenueAddress { get; private set; }
        public string? CoverImageUrl { get; private set; }

        public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();
        public IReadOnlyCollection<EventReview> Reviews => _reviews.AsReadOnly();
        public IReadOnlyCollection<Preference> Preferences => _preferences.AsReadOnly();

        private Event(
            Guid id,
            Guid organizerId,
            string title,
            DateTime startTime,
            DateTime endTime,
            Address venueAddress,
            string? coverImageUrl) : base(id)
        {
            OrganizerId = organizerId;
            Title = title;
            StartTime = startTime;
            EndTime = endTime;
            VenueAddress = venueAddress;
            CoverImageUrl = coverImageUrl;
            Status = EventStatus.Draft;
        }

        private Event() : base(Guid.Empty)
        {
            Title = null!;
            VenueAddress = null!;
            Status = EventStatus.Draft;
        }

        public static Result<Event> Create(
            Guid organizerId,
            string title,
            DateTime startTime,
            DateTime endTime,
            Address venueAddress,
            string? coverImageUrl = null,
            Guid? id = null)
        {
            if (organizerId == Guid.Empty)
                return Result.Failure<Event>(Error.Create("Event.OrganizerIdRequired", "Organizer ID is required."));

            if (string.IsNullOrWhiteSpace(title))
                return Result.Failure<Event>(Error.Create("Event.TitleRequired", "Event title is required."));

            if (title.Length > Constants.Event.TitleMaxLength)
                return Result.Failure<Event>(Error.Create("Event.TitleTooLong", $"Event title cannot exceed {Constants.Event.TitleMaxLength} characters."));

            if (endTime <= startTime)
                return Result.Failure<Event>(Error.Create("Event.InvalidTimeRange", "End time must be after start time."));

            if (!string.IsNullOrWhiteSpace(coverImageUrl) && coverImageUrl.Length > Constants.Event.CoverImageUrlMaxLength)
                return Result.Failure<Event>(Error.Create("Event.CoverImageUrlTooLong", $"Cover image URL cannot exceed {Constants.Event.CoverImageUrlMaxLength} characters."));

            var evt = new Event(
                id ?? Guid.NewGuid(),
                organizerId,
                title.Trim(),
                startTime,
                endTime,
                venueAddress,
                coverImageUrl?.Trim());

            return evt;
        }

        public Result Publish()
        {
            if (Status != EventStatus.Draft)
                return Result.Failure(Error.Create("Event.InvalidStatusTransition", "Only draft events can be published."));

            Status = EventStatus.Published;
            return Result.Success();
        }

        public Result Complete()
        {
            if (Status != EventStatus.Published)
                return Result.Failure(Error.Create("Event.InvalidStatusTransition", "Only published events can be completed."));

            Status = EventStatus.Completed;
            return Result.Success();
        }

        public Result Cancel()
        {
            if (Status == EventStatus.Completed)
                return Result.Failure(Error.Create("Event.InvalidStatusTransition", "Completed events cannot be canceled."));

            Status = EventStatus.Canceled;
            return Result.Success();
        }

        public Result<Seat> AddSeat(int row, int seatNumber, Money price, string? type = null)
        {
            var seatResult = Seat.Create(Id, row, seatNumber, price, type);
            if (seatResult.IsFail)
                return seatResult;

            _seats.Add(seatResult.Value!);
            return seatResult;
        }

        public Result AddPreference(Preference preference)
        {
            if (_preferences.Any(p => p.Id == preference.Id))
                return Result.Failure(Error.Create("Event.DuplicatePreference", "Preference already exists for this event."));

            _preferences.Add(preference);
            return Result.Success();
        }
    }
}

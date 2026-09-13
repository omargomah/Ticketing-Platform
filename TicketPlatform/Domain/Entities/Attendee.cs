using Domain.Interfaces;
using Domain.Shared;

namespace Domain.Entities
{
    public class Attendee : Entity, IAggregateRoot
    {
        private readonly List<Booking> _bookings = new();
        private readonly List<EventReview> _reviews = new();
        private readonly List<Preference> _preferences = new();

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime? DateOfBirth { get; private set; }

        public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();
        public IReadOnlyCollection<EventReview> Reviews => _reviews.AsReadOnly();
        public IReadOnlyCollection<Preference> Preferences => _preferences.AsReadOnly();

        private Attendee(
            Guid id,
            string firstName,
            string lastName,
            DateTime? dateOfBirth) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
        }

        private Attendee() : base(Guid.Empty)
        {
            FirstName = null!;
            LastName = null!;
        }

        public static Result<Attendee> Create(
            string firstName,
            string lastName,
            DateTime? dateOfBirth = null,
            Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<Attendee>(Error.Create("Attendee.FirstNameRequired", "First name is required."));

            if (firstName.Length > Constants.Attendee.FirstNameMaxLength)
                return Result.Failure<Attendee>(Error.Create("Attendee.FirstNameTooLong", $"First name cannot exceed {Constants.Attendee.FirstNameMaxLength} characters."));

            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure<Attendee>(Error.Create("Attendee.LastNameRequired", "Last name is required."));

            if (lastName.Length > Constants.Attendee.LastNameMaxLength)
                return Result.Failure<Attendee>(Error.Create("Attendee.LastNameTooLong", $"Last name cannot exceed {Constants.Attendee.LastNameMaxLength} characters."));

            if (dateOfBirth.HasValue && dateOfBirth.Value > DateTime.UtcNow)
                return Result.Failure<Attendee>(Error.Create("Attendee.InvalidDateOfBirth", "Date of birth cannot be in the future."));

            return new Attendee(
                id ?? Guid.NewGuid(),
                firstName.Trim(),
                lastName.Trim(),
                dateOfBirth);
        }

        public Result AddPreference(Preference preference)
        {
            if (_preferences.Any(p => p.Id == preference.Id))
                return Result.Failure(Error.Create("Attendee.DuplicatePreference", "Preference already exists for this attendee."));

            _preferences.Add(preference);
            return Result.Success();
        }
    }
}

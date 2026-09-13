using Domain.Shared;

namespace Domain.Entities
{
    public class Preference : Entity
    {
        private readonly List<Event> _events = new();
        private readonly List<Attendee> _attendees = new();

        public string Name { get; private set; }

        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();
        public IReadOnlyCollection<Attendee> Attendees => _attendees.AsReadOnly();

        private Preference(Guid id, string name) : base(id)
        {
            Name = name;
        }

        private Preference() : base(Guid.Empty)
        {
            Name = null!;
        }

        public static Result<Preference> Create(string name, Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Preference>(Error.Create("Preference.NameRequired", "Preference name is required."));

            if (name.Length > Constants.Preference.NameMaxLength)
                return Result.Failure<Preference>(Error.Create("Preference.NameTooLong", $"Preference name cannot exceed {Constants.Preference.NameMaxLength} characters."));

            return new Preference(id ?? Guid.NewGuid(), name.Trim());
        }
    }
}

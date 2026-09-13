using Domain.Interfaces;
using Domain.Shared;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Organizer : Entity, IAggregateRoot
    {
        private readonly List<Event> _events = new();

        public string Name { get; private set; }
        public TaxRegistrationNumber TaxRegistrationNumber { get; private set; }
        public string? WebSite { get; private set; }
        public Money TotalRevenue { get; private set; }
        public BankIban BankIban { get; private set; }

        public IReadOnlyCollection<Event> Events => _events.AsReadOnly();

        private Organizer(
            Guid id,
            string name,
            TaxRegistrationNumber taxRegistrationNumber,
            string? webSite,
            Money totalRevenue,
            BankIban bankIban) : base(id)
        {
            Name = name;
            TaxRegistrationNumber = taxRegistrationNumber;
            WebSite = webSite;
            TotalRevenue = totalRevenue;
            BankIban = bankIban;
        }

        private Organizer() : base(Guid.Empty)
        {
            Name = null!;
            TaxRegistrationNumber = null!;
            BankIban = null!;
            TotalRevenue = Money.Zero;
        }

        public static Result<Organizer> Create(
            string name,
            TaxRegistrationNumber taxRegistrationNumber,
            BankIban bankIban,
            string? webSite = null,
            Money? initialRevenue = null,
            Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Organizer>(Error.Create("Organizer.NameRequired", "Organizer name is required."));

            if (name.Length > Constants.Organizer.NameMaxLength)
                return Result.Failure<Organizer>(Error.Create("Organizer.NameTooLong", $"Organizer name cannot exceed {Constants.Organizer.NameMaxLength} characters."));

            if (!string.IsNullOrWhiteSpace(webSite) && webSite.Length > Constants.Organizer.WebSiteMaxLength)
                return Result.Failure<Organizer>(Error.Create("Organizer.WebSiteTooLong", $"Website URL cannot exceed {Constants.Organizer.WebSiteMaxLength} characters."));

            var organizer = new Organizer(
                id ?? Guid.NewGuid(),
                name.Trim(),
                taxRegistrationNumber,
                webSite?.Trim(),
                initialRevenue ?? Money.Zero,
                bankIban);

            return organizer;
        }

        public Result RecordRevenue(Money revenue)
        {
            TotalRevenue = TotalRevenue.Add(revenue);
            return Result.Success();
        }

        public Result UpdateProfile(string name, string? webSite, BankIban bankIban)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure(Error.Create("Organizer.NameRequired", "Organizer name is required."));

            if (name.Length > Constants.Organizer.NameMaxLength)
                return Result.Failure(Error.Create("Organizer.NameTooLong", $"Organizer name cannot exceed {Constants.Organizer.NameMaxLength} characters."));

            if (!string.IsNullOrWhiteSpace(webSite) && webSite.Length > Constants.Organizer.WebSiteMaxLength)
                return Result.Failure(Error.Create("Organizer.WebSiteTooLong", $"Website URL cannot exceed {Constants.Organizer.WebSiteMaxLength} characters."));

            Name = name.Trim();
            WebSite = webSite?.Trim();
            BankIban = bankIban;

            return Result.Success();
        }
    }
}

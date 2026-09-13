using Domain.Shared;

namespace Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public string VenueName { get; }
        public string Street { get; }
        public string City { get; }

        private Address(string venueName, string street, string city)
        {
            VenueName = venueName;
            Street = street;
            City = city;
        }

        public static Result<Address> Create(string venueName, string street, string city)
        {
            if (string.IsNullOrWhiteSpace(venueName))
                return Result.Failure<Address>(Error.Create("Address.VenueNameRequired", "Venue name is required."));

            if (string.IsNullOrWhiteSpace(street))
                return Result.Failure<Address>(Error.Create("Address.StreetRequired", "Street is required."));

            if (string.IsNullOrWhiteSpace(city))
                return Result.Failure<Address>(Error.Create("Address.CityRequired", "City is required."));

            if (venueName.Length > Constants.Address.VenueNameMaxLength)
                return Result.Failure<Address>(Error.Create("Address.VenueNameTooLong", $"Venue name cannot exceed {Constants.Address.VenueNameMaxLength} characters."));

            if (street.Length > Constants.Address.StreetMaxLength)
                return Result.Failure<Address>(Error.Create("Address.StreetTooLong", $"Street cannot exceed {Constants.Address.StreetMaxLength} characters."));

            if (city.Length > Constants.Address.CityMaxLength)
                return Result.Failure<Address>(Error.Create("Address.CityTooLong", $"City cannot exceed {Constants.Address.CityMaxLength} characters."));

            return new Address(venueName, street, city);
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return VenueName;
            yield return Street;
            yield return City;
        }
    }
}

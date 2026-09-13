using Domain.Shared;

namespace Domain.ValueObjects
{
    public class TaxRegistrationNumber : ValueObject
    {
        public string Value { get; }

        private TaxRegistrationNumber(string value)
        {
            Value = value;
        }

        public static Result<TaxRegistrationNumber> Create(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return Result.Failure<TaxRegistrationNumber>(Error.Create("TaxRegistrationNumber.Empty", "Tax Registration Number cannot be empty."));

            string trimmed = number.Trim();

            if (trimmed.Length != Constants.TaxRegistrationNumber.RequiredLength || !trimmed.All(char.IsDigit))
                return Result.Failure<TaxRegistrationNumber>(Error.Create("TaxRegistrationNumber.InvalidFormat", $"Tax Registration Number must be exactly {Constants.TaxRegistrationNumber.RequiredLength} numeric digits."));

            return new TaxRegistrationNumber(trimmed);
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}

using Domain.Shared;

namespace Domain.ValueObjects
{
    public class BankIban : ValueObject
    {
        public string Value { get; }

        private BankIban(string value)
        {
            Value = value;
        }

        public static Result<BankIban> Create(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban))
                return Result.Failure<BankIban>(Error.Create("BankIban.Empty", "Bank IBAN cannot be empty."));

            string cleaned = iban.Replace(" ", "").ToUpperInvariant();

            if (cleaned.Length != Constants.BankIban.RequiredLength)
                return Result.Failure<BankIban>(Error.Create("BankIban.InvalidLength", $"Bank IBAN must be exactly {Constants.BankIban.RequiredLength} characters long."));

            return new BankIban(cleaned);
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}

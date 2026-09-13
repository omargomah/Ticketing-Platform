using Domain.Shared;

namespace Domain.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; }

        private Money(decimal amount)
        {
            Amount = amount;
        }

        public static Result<Money> Create(decimal amount)
        {
            if (amount < Constants.Money.MinAmount)
                return Result.Failure<Money>(Error.Create("Money.Negative", "Money amount cannot be negative."));

            return new Money(amount);
        }

        public static Money Zero => new(Constants.Money.MinAmount);

        public Money Add(Money other)
        {
            return new Money(Amount + other.Amount);
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Amount;
        }
    }
}

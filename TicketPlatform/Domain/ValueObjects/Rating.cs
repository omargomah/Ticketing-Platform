using Domain.Shared;

namespace Domain.ValueObjects
{
    public class Rating : ValueObject
    {
        public byte Value { get; }

        private Rating(byte value)
        {
            Value = value;
        }

        public static Result<Rating> Create(byte value)
        {
            if (value < Constants.Rating.MinScore || value > Constants.Rating.MaxScore)
                return Result.Failure<Rating>(Error.Create("Rating.OutOfRange", $"Rating score must be between {Constants.Rating.MinScore} and {Constants.Rating.MaxScore}."));

            return new Rating(value);
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}

namespace Domain.ValueObjects
{
    public abstract class ValueObject:IEquatable<ValueObject>
    {
        public bool Equals(ValueObject? other)
        {
            if (other is null)
                return false;
          return ValuesAreEqual(other);            

        }
        public override bool Equals(object? obj)
        {
            if (obj is not ValueObject valueObject)
                return false;
          return ValuesAreEqual(other: valueObject);            
        }
        public bool ValuesAreEqual(ValueObject other)
        {
            return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
        }
        public override int GetHashCode()
        {
            return GetAtomicValues().Aggregate(default(int), HashCode.Combine);
        }
        public abstract IEnumerable<object> GetAtomicValues();

    }
}

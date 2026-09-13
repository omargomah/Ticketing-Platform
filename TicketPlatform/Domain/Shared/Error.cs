namespace Domain.Shared
{
    public class Error : IEquatable<Error>
    {
        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");
        protected Error(string Code, string message)
        {
            Message = message;
            this.Code = Code;
        }
        public static Error Create(string Code, string message) => new(Code, message);

        public string Message { get; }
        public string Code { get; }

        public bool Equals(Error? other)
        {
            if (other is null)
                return false;
            if (other.GetType() != GetType())
                return false;
            return other.Code == Code && other.Message == other.Message;
        }
        public override bool Equals(object? obj)
        {
            if (obj is not Error error)
                return false;
            return Equals(other: error);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Code.GetHashCode() * 41, Message.GetHashCode() * 17) * 23;
        }
        public static bool operator ==(Error? right  ,Error? left)
        {
            if (right is null && left is null)
                return true;
            if (right is null || left is null)
                return false;
            return left.Equals(other: right);
        }
        public static bool operator !=(Error? right, Error? left)
        {
            return !(right == left);
        }
    }
}

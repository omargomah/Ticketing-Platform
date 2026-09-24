using System.Text.Json.Serialization;

namespace Domain.Shared
{
    public class Result
    {
        protected Result(Error? error, bool isSuccess)
        {
            if(isSuccess && error != null )
                throw new InvalidOperationException("A success result cannot have an error state.");
            
            if(!isSuccess && error == null)
                throw new InvalidOperationException("A failure result must provide an error reason.");
            
            Error = error;
            IsSuccess = isSuccess;
        }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Error? Error { get; set; }
        public bool IsSuccess { get;}
        [JsonIgnore]
        public bool IsFail => !IsSuccess;
        public static Result Success() => new(null, true);
        public static Result Failure(Error error) => new(error, false);
        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, null);
        public static Result<TValue> Failure<TValue>(Error error) => new(default,false, error);
        public static Result<TValue> Create<TValue>(TValue? value) =>value is null?  Failure<TValue>(Error.NullValue) : Success<TValue>(value);
    }
}

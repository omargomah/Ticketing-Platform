namespace Domain.Shared
{
    public class Result
    {
        protected Result(Error error, bool isSuccess)
        {
            if(isSuccess && error != Error.None )
                throw new InvalidOperationException("A success result cannot have an error state.");
            
            if(!isSuccess && error == Error.None)
                throw new InvalidOperationException("A failure result must provide an error reason.");
            
            Error = error;
            IsSuccess = isSuccess;
        }

        public Error Error { get; set; }
        public bool IsSuccess { get;}
        public bool IsFail => !IsSuccess;
        public static Result Success() => new(Error.None, true);
        public static Result Failure(Error error) => new(error, true);
        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
        public static Result<TValue> Failure<TValue>(Error error) => new(default,false, error);
        public static Result<TValue> Create<TValue>(TValue? value) =>value is null?  Failure<TValue>(Error.NullValue) : Success<TValue>(value);
    }
}

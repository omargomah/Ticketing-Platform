using System.Text.Json.Serialization;

namespace Domain.Shared
{
    public class Result<TValue> :Result
    {
        private readonly TValue? _value;
        protected internal Result(TValue? value,bool isSuccess,Error? error):base(error,isSuccess)
        {
            _value = value;
        }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TValue? Value => IsSuccess ? _value : default /*throw new InvalidOperationException("The value of a failure result cannot be accessed.")*/;
        
        public static implicit operator Result<TValue>(TValue? value) => Create(value); 
    }
}

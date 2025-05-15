namespace BuildingBlocks.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }
        protected Result(bool isSuccess, string error = null)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true);
        public static Result Failure(string error) => new(false, error);
        public static Result<T> Success<T>(T value) => new(value, true);
        public static Result<T> Failure<T>(string error) => new(default, false, error);
    }

    public class Result<T> : Result
    {
        private readonly T _value;
        public T Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("Cannot access Value when Result is failure.");

        protected internal Result(T value, bool isSuccess, string error = null)
            : base(isSuccess, error)
        {
            _value = value;
        }
    }
}

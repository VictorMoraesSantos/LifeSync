namespace BuildingBlocks.Results
{
    public record Error
    {
        public string Description { get; }
        public ErrorType Type { get; }

        public Error(string description, ErrorType type)
        {
            Description = description;
            Type = type;
        }

        public static readonly Error None = new Error(string.Empty, ErrorType.Failure);
        public static readonly Error NullValue = new Error("Null value was provided", ErrorType.Failure);

        public static Error Failure(string description) => new Error(description, ErrorType.Failure);
        public static Error NotFound(string description) => new Error(description, ErrorType.NotFound);
        public static Error Problem(string description) => new Error(description, ErrorType.Problem);
        public static Error Conflict(string description) => new Error(description, ErrorType.Conflict);
    }

}

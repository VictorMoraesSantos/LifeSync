namespace BuildingBlocks.Results
{
    public record Error
    {
        public static readonly Error None = new(string.Empty, ErrorType.Failure);
        public static readonly Error NullValue = new(
            "Null value was provided",
            ErrorType.Failure);

        public Error(string description, ErrorType type)
        {
            Description = description;
            Type = type;
        }

        public string Description { get; }
        public ErrorType Type { get; }

        public static Error Failure(string description) => new(description, ErrorType.Failure);
        public static Error NotFound(string description) => new(description, ErrorType.NotFound);
        public static Error Problem(string description) => new(description, ErrorType.Problem);
        public static Error Conflict(string description) => new(description, ErrorType.Conflict);
    }

}

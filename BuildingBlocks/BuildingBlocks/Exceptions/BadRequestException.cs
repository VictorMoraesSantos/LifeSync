namespace BuildingBlocks.Exceptions
{
    public class BadRequestException : Exception
    {
        public string? Details { get; }
        public IEnumerable<string>? Errors { get; } 

        public BadRequestException()
        { }

        public BadRequestException(string message) : base(message)
        { }

        public BadRequestException(string message, string details) : base(message)
        {
            Details = details;
        }

        public BadRequestException(string message, IEnumerable<string> errors) : base(message)
        {
            Errors = errors;
        }
    }
}

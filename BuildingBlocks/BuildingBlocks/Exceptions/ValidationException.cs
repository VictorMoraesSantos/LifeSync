using BuildingBlocks.Validation;

namespace BuildingBlocks.Exceptions
{
    public class ValidationException : Exception
    {
        public List<ValidationError> Errors { get; }

        public ValidationException(List<ValidationError> errors)
            : base("One or more validation failures have occurred.")
        {
            Errors = errors;
        }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new List<ValidationError>();
        }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            Errors = new List<ValidationError>();
        }
    }
}
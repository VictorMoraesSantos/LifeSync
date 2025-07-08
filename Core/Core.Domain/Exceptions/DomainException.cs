using BuildingBlocks.Results;

namespace Core.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public string Description { get; }

        public DomainException(string message) : base(message)
        {
            Description = "Domain.Error";
        }

        public DomainException(Error error) : base(error.Description)
        {
            Description = error.Description;
        }
    }
}

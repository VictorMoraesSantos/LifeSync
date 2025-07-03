using BuildingBlocks.Results;

namespace Core.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public string Code { get; }

        public DomainException(string message) : base(message)
        {
            Code = "Domain.Error";
        }

        public DomainException(Error error) : base(error.Description)
        {
            Code = error.Code;
        }
    }
}

using BuildingBlocks.Results;

namespace Users.Domain.Errors
{
    public static class ContactErrors
    {
        public static Error InvalidFormat => Error.Failure("O formato do email não é válido");
    }
}

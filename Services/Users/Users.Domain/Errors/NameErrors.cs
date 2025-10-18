using BuildingBlocks.Results;

namespace Users.Domain.Errors
{
    public static class NameErrors
    {
        public static Error NullName => Error.Failure("O nome não pode ser nulo ou vazio");
    }
}

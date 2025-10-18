using BuildingBlocks.CQRS.Commands;

namespace Users.Application.Features.Users.Commands.UpdateUser
{
    public record UpdateUserCommand(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        DateOnly? BirthDate)
        : ICommand<UpdateUserCommandResult>;
    public record UpdateUserCommandResult(bool Success);
}

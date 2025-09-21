using BuildingBlocks.CQRS.Request;

namespace Users.Application.Features.Users.Commands.UpdateUser
{
    public record UpdateUserCommand(
        string Id,
        string FirstName,
        string LastName,
        string Email,
        DateOnly? BirthDate)
        : IRequest<UpdateUserCommandResponse>;
    public record UpdateUserCommandResponse(bool Success);
}

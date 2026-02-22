using BuildingBlocks.CQRS.Requests.Commands;

namespace Users.Application.Features.Users.Commands.DeleteUser
{
    public record DeleteUserCommand(string userId) : ICommand<DeleteUserResult>;
    public record DeleteUserResult(bool IsSuccess);
}

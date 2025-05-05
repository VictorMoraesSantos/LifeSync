using MediatR;

namespace Users.Application.Users.Commands.DeleteUser
{
    public record DeleteUserCommand(string userId) : IRequest<DeleteUserResponse>;
    public record DeleteUserResponse(bool IsSuccess);
}

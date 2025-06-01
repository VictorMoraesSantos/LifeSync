using BuildingBlocks.CQRS.Request;

namespace Users.Application.Features.Users.Commands.DeleteUser
{
    public record DeleteUserCommand(string userId) : IRequest<DeleteUserResponse>;
    public record DeleteUserResponse(bool IsSuccess);
}

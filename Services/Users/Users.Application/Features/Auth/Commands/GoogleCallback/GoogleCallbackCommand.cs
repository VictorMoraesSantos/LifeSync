using BuildingBlocks.CQRS.Requests.Commands;
using Users.Application.DTOs.Auth;

namespace Users.Application.Features.Auth.Commands.GoogleCallback
{
    public record GoogleCallbackCommand(string Code, string? State) : ICommand<AuthResult>;
}

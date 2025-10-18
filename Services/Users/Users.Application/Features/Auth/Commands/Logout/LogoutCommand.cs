using BuildingBlocks.CQRS.Commands;
using System.Security.Claims;

namespace Users.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand(ClaimsPrincipal User) : ICommand<LogoutResult>;
    public record LogoutResult(bool IsSuccess);
}

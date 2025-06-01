using BuildingBlocks.CQRS.Request;
using System.Security.Claims;

namespace Users.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand(ClaimsPrincipal User) : IRequest;
}

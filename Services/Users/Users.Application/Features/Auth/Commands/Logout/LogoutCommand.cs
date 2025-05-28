using MediatR;
using System.Security.Claims;

namespace Users.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand(ClaimsPrincipal User) : IRequest<Unit>;
}

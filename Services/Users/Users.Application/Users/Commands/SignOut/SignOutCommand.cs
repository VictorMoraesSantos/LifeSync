using MediatR;
using System.Security.Claims;

namespace Users.Application.Users.Commands.LogOut
{
    public record SignOutCommand(ClaimsPrincipal User) : IRequest<Unit>;
}

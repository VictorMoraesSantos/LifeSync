using MediatR;
using System.Security.Claims;

namespace Users.Application.Users.Commands.ChangePassword
{
    public record ChangePasswordCommand(ClaimsPrincipal User, string CurrentPassword, string NewPassword) : IRequest<bool>;
}

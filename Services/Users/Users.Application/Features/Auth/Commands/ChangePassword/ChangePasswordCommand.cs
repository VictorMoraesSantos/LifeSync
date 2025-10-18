using BuildingBlocks.CQRS.Commands;
using System.Security.Claims;

namespace Users.Application.Features.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommand(ClaimsPrincipal User, string CurrentPassword, string NewPassword) : ICommand;
}

using BuildingBlocks.CQRS.Request;
using System.Security.Claims;

namespace Users.Application.Features.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommand(ClaimsPrincipal User, string CurrentPassword, string NewPassword) : IRequest;
}

using MediatR;

namespace Users.Application.Features.Auth.Commands.ResetPassword
{
    public record ResetPasswordCommand(string UserId, string Token, string NewPassword) : IRequest<bool>;
}

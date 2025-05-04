using MediatR;

namespace Users.Application.Auth.Commands.ResetPassword
{
    public record ResetPasswordCommand(string UserId, string Token, string NewPassword) : IRequest<bool>;
}

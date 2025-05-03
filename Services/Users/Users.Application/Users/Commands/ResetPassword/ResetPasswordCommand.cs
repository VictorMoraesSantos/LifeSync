using MediatR;

namespace Users.Application.Users.Commands.ResetPassword
{
    public record ResetPasswordCommand(string UserId, string Token, string NewPassword) : IRequest<bool>;
}

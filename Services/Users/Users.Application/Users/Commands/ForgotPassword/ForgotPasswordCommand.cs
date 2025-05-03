using MediatR;

namespace Users.Application.Users.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<bool>;
}

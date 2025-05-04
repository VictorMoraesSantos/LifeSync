using MediatR;

namespace Users.Application.Auth.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest;
}

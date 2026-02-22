using BuildingBlocks.CQRS.Requests.Commands;

namespace Users.Application.Features.Auth.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : ICommand;
}

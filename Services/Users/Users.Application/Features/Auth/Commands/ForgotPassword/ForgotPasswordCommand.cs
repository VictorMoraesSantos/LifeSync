using BuildingBlocks.CQRS.Request;

namespace Users.Application.Features.Auth.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest;
}

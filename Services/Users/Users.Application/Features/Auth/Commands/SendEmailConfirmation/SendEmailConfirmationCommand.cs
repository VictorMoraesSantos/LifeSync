using MediatR;

namespace Users.Application.Features.Auth.Commands.SendEmailConfirmation
{
    public record SendEmailConfirmationCommand(string Email) : IRequest;
}

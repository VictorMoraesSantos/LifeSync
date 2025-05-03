using MediatR;

namespace Users.Application.Users.Commands.ResendEmailConfirmation
{
    public record ResendEmailConfirmationCommand(string Email): IRequest<bool>;
}

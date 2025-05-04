using MediatR;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;

namespace Users.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserCommandResponse>
    {
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UpdateUserCommandResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            UpdateUserDTO dto = new(
                command.Id,
                command.FirstName,
                command.LastName,
                command.Email,
                command.BirthDate);

            bool result = await _userService.UpdateUserProfileAsync(dto);
            UpdateUserCommandResponse response = new(result);
            return response;
        }
    }
}

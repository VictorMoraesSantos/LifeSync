using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;

namespace Users.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, UpdateUserCommandResult>
    {
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Result<UpdateUserCommandResult>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateUserDTO(
                command.Id,
                command.FirstName,
                command.LastName,
                command.Email,
                command.BirthDate);

            var result = await _userService.UpdateUserProfileAsync(dto);
            if(!result.IsSuccess)
                return Result.Failure<UpdateUserCommandResult>(result.Error!);

            return Result.Success(new UpdateUserCommandResult(result.Value));
        }
    }
}

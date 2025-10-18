using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Features.Users.Commands.UpdateUser;
using Users.Application.Features.Users.Queries.GetAllUsers;
using Users.Application.Features.Users.Queries.GetUser;

namespace Users.API.Controllers
{
    public class UsersController : ApiController
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{userId:int}")]
        public async Task<HttpResult<object>> GetUser(string userId, CancellationToken cancellationToken)
        {
            var query = new GetUserQuery(userId);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAllUsers(CancellationToken cancellationToken)
        {
            var query = new GetAllUsersQuery();
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpPut("{userId:int}")]
        public async Task<HttpResult<object>> UpdateUser(int userId, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var updateCommand = new UpdateUserCommand(userId, command.FirstName, command.LastName, command.Email, command.BirthDate);
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest("Could not update user");
        }
    }
}

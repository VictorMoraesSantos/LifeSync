using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Features.Users.Commands.UpdateUser;
using Users.Application.Features.Users.Queries.GetAllUsers;
using Users.Application.Features.Users.Queries.GetUser;
using BuildingBlocks.CQRS.Sender;

namespace Users.API.Controllers
{
    public class UserController : ApiController
    {
        private readonly ISender _sender;

        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(string userId)
        {
            GetUserQuery request = new(userId);
            GetUserQueryResponse result = await _sender.Send(request);
            return Ok(result);
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllUsers()
        {
            GetAllUsersQuery request = new();
            GetAllUsersQueryResponse result = await _sender.Send(request);
            return Ok(result);
        }

        [HttpPut("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand request)
        {
            UpdateUserCommandResponse result = await _sender.Send(request);
            return Ok(result);
        }
    }
}

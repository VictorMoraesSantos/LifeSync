using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Users.Commands.UpdateUser;
using Users.Application.Users.Queries.GetUser;
using Users.Application.Users.Queries.GetUsers;

namespace Users.API.Controllers
{
    public class UserController : ApiController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(string userId)
        {
            GetUserQuery request = new(userId);
            GetUserQueryResponse result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllUsers()
        {
            GetAllUsersQuery request = new();
            GetAllUsersQueryResponse result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPut("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand request)
        {
            UpdateUserCommandResponse result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}

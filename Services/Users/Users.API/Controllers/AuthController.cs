using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.DTOs.Auth;
using Users.Application.Users.Commands.LogIn;
using Users.Application.Users.Commands.Register;

namespace Users.API.Controllers
{
    public class AuthController : ApiController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] SignUpCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> LogIn([FromBody] SignInCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}

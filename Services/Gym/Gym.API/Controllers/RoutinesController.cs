using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Gym.Application.Features.Routine.Commands.Create;
using Gym.Application.Features.Routine.Commands.Delete;
using Gym.Application.Features.Routine.Commands.Update;
using Gym.Application.Features.Routine.Queries.GetAll;
using Gym.Application.Features.Routine.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace Gym.API.Controllers
{
    public class RoutinesControlle : ApiController
    {
        private readonly ISender _sender;

        public RoutinesControlle(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetRoutineByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Routine)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll([FromQuery] GetAllRoutinesQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Routines)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create([FromBody] CreateRoutineCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!.Id)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update(int id, [FromBody] UpdateRoutineCommand command, CancellationToken cancellationToken)
        {
            var updatedCommand = new UpdateRoutineCommand(id, command.Name, command.Description);
            var result = await _sender.Send(updatedCommand, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteRoutineCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }
    }
}

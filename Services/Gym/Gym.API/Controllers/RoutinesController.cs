using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Gym.Application.Features.Routine.Commands.CreateRoutine;
using Gym.Application.Features.Routine.Commands.DeleteRoutine;
using Gym.Application.Features.Routine.Commands.UpdateRoutine;
using Gym.Application.Features.Routine.Queries.GetAllRoutines;
using Gym.Application.Features.Routine.Queries.GetRoutineById;
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
        public async Task<HttpResult<GetRoutineByIdResult>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetRoutineByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<GetRoutineByIdResult>.Ok(result.Value!)
                : HttpResult<GetRoutineByIdResult>.BadRequest(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllRoutinesResult>> GetAll([FromQuery] GetAllRoutinesQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<GetAllRoutinesResult>.Ok(result.Value!)
                : HttpResult<GetAllRoutinesResult>.BadRequest(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<CreateRoutineResult>> Create([FromBody] CreateRoutineCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<CreateRoutineResult>.Created(result.Value!)
                : HttpResult<CreateRoutineResult>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateRoutineCommand>> Update(int id, [FromBody] UpdateRoutineCommand command, CancellationToken cancellationToken)
        {
            var updatedCommand = new UpdateRoutineCommand(id, command.Name, command.Description);
            var result = await _sender.Send(updatedCommand, cancellationToken);
            return result.IsSuccess
                ? HttpResult<UpdateRoutineCommand>.Updated()
                : HttpResult<UpdateRoutineCommand>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteRoutineCommand>> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteRoutineCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<DeleteRoutineCommand>.Deleted()
                : HttpResult<DeleteRoutineCommand>.BadRequest(result.Error!.Description);
        }
    }
}

using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Gym.Application.Features.RoutineExercise.Commands.Create;
using Gym.Application.Features.RoutineExercise.Commands.Delete;
using Gym.Application.Features.RoutineExercise.Commands.Update;
using Gym.Application.Features.RoutineExercise.Queries.GetAll;
using Gym.Application.Features.RoutineExercise.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace Gym.API.Controllers
{
    [Route("api/routine-exercises")]
    public class RoutineExercisesController : ApiController
    {
        private readonly ISender _sender;

        public RoutineExercisesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetRoutineExerciseByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllRoutineExercisesQuery();
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create([FromBody] CreateRoutineExerciseCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                 ? HttpResult<object>.Created(result.Value!)
                 : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update(int id, [FromBody] UpdateRoutineExerciseCommand command, CancellationToken cancellationToken)
        {
            var updatedCommand = new UpdateRoutineExerciseCommand(
                id,
                command.Sets,
                command.Repetitions,
                command.RestBetweenSets,
                command.RecommendedWeight,
                command.Instructions);
            var result = await _sender.Send(updatedCommand, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteRoutineExerciseCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }
    }
}

using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Gym.Application.DTOs.Exercise;
using Gym.Application.Features.Exercise.Commands.Create;
using Gym.Application.Features.Exercise.Commands.Delete;
using Gym.Application.Features.Exercise.Commands.Update;
using Gym.Application.Features.Exercise.Queries.GetAll;
using Gym.Application.Features.Exercise.Queries.GetByFilter;
using Gym.Application.Features.Exercise.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace Gym.API.Controllers
{
    public class ExercisesController : ApiController
    {
        private readonly ISender _sender;

        public ExercisesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetExerciseByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Exercise)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll([FromQuery] GetAllExercisesQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Exercises)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpGet("search")]
        public async Task<HttpResult<object>> Search([FromQuery] ExerciseFilterDTO filter, CancellationToken cancellationToken)
        {
            var query = new GetExerciseByFilterQuery(filter);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value?.Items!, result.Value?.Pagination!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create([FromBody] CreateExerciseCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!.Id)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update(int id, [FromBody] UpdateExerciseCommand command, CancellationToken cancellationToken)
        {
            var updatedCommand = new UpdateExerciseCommand(id, command.Name, command.Description, command.MuscleGroup, command.ExerciseType, command.EquipmentType);
            var result = await _sender.Send(updatedCommand, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteExerciseCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }
    }
}

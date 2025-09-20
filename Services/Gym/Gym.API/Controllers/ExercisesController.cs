using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Gym.Application.Features.Exercise.Commands.CreateExercise;
using Gym.Application.Features.Exercise.Commands.DeleteExerciseCommand;
using Gym.Application.Features.Exercise.Commands.UpdateExercise;
using Gym.Application.Features.Exercise.Queries.GetAll;
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
        public async Task<HttpResult<GetExerciseByIdResult>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetExerciseByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<GetExerciseByIdResult>.Ok(result.Value!)
                : HttpResult<GetExerciseByIdResult>.BadRequest(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllExercisesResult>> GetAll([FromQuery] GetAllExercisesQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<GetAllExercisesResult>.Ok(result.Value!)
                : HttpResult<GetAllExercisesResult>.BadRequest(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<CreateExerciseResult>> Create([FromBody] CreateExerciseCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<CreateExerciseResult>.Created(result.Value!)
                : HttpResult<CreateExerciseResult>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateExerciseCommand>> Update(int id, [FromBody] UpdateExerciseCommand command, CancellationToken cancellationToken)
        {
            var updatedCommand = new UpdateExerciseCommand(id, command.Name, command.Description, command.MuscleGroup, command.ExerciseType, command.EquipmentType);
            var result = await _sender.Send(updatedCommand, cancellationToken);
            return result.IsSuccess
                ? HttpResult<UpdateExerciseCommand>.Updated()
                : HttpResult<UpdateExerciseCommand>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteExerciseCommand>> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteExerciseCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<DeleteExerciseCommand>.Deleted()
                : HttpResult<DeleteExerciseCommand>.BadRequest(result.Error!.Description);
        }
    }
}

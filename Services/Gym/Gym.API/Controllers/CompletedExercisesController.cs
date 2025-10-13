using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Gym.Application.Features.CompletedExercise.Commands.CreateCompletedExercise;
using Gym.Application.Features.CompletedExercise.Commands.DeleteCompletedExercise;
using Gym.Application.Features.CompletedExercise.Commands.UpdateCompletedExercise;
using Gym.Application.Features.CompletedExercise.Queries.GetAllCompletedExercises;
using Gym.Application.Features.CompletedExercise.Queries.GetCompletedExerciseById;
using Microsoft.AspNetCore.Mvc;

namespace Gym.API.Controllers
{
    [Route("api/completed-exercises")]
    public class CompletedExercisesController : ApiController
    {
        private readonly ISender _sender;

        public CompletedExercisesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetCompletedExerciseByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllCompletedExercisesQuery();
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create(CreateCompletedExerciseCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update(int id, UpdateCompletedExerciseCommand command, CancellationToken cancellationToken)
        {
            var updatedCommmand = new UpdateCompletedExerciseCommand(
                id,
                command.SetsCompleted,
                command.RepetitionsCompleted,
                command.RestBetweenSets,
                command.WeightUsed,
                command.Notes);
            var result = await _sender.Send(updatedCommmand, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteCompletedExerciseCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }
    }
}

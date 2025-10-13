using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Gym.Application.Features.TrainingSession.Commands.Create;
using Gym.Application.Features.TrainingSession.Commands.Delete;
using Gym.Application.Features.TrainingSession.Commands.Update;
using Gym.Application.Features.TrainingSession.Queries.GetAll;
using Gym.Application.Features.TrainingSession.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace Gym.API.Controllers
{
    [Route("api/training-sessions")]
    public class TrainingSessionsController : ApiController
    {
        private readonly ISender _sender;

        public TrainingSessionsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetTrainingSessionByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllTrainingSessionsQuery();
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create(CreateTrainingSessionCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update(int id, UpdateTrainingSessionCommand command, CancellationToken cancellation)
        {
            var updatedCommand = new UpdateTrainingSessionCommand(
                id,
                command.RoutineId,
                command.StartTime,
                command.EndTime,
                command.Notes);
            var result = await _sender.Send(updatedCommand, cancellation);
            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteTrainingSessionsCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }
    }
}

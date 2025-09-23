using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.Features.TaskLabels.Commands.CreateTaskLabel;
using TaskManager.Application.Features.TaskLabels.Commands.DeleteTaskLabel;
using TaskManager.Application.Features.TaskLabels.Commands.UpdateTaskLabel;
using TaskManager.Application.Features.TaskLabels.Queries.GetAll;
using TaskManager.Application.Features.TaskLabels.Queries.GetByFilter;
using TaskManager.Application.Features.TaskLabels.Queries.GetById;
using TaskManager.Application.Features.TaskLabels.Queries.GetByUser;

namespace TaskManager.API.Controllers
{
    [Route("api/v1/task-labels")]
    public class TaskLabelsController : ApiController
    {
        private readonly ISender _sender;

        public TaskLabelsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetTaskLabelByIdResult>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetTaskLabelByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<GetTaskLabelByIdResult>.Ok(result.Value!)
                : HttpResult<GetTaskLabelByIdResult>.NotFound(result.Error!.Description);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<GetByUserResponse>> GetByUserId(int userId, CancellationToken cancellationToken)
        {
            var query = new GetByUserQuery(userId);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<GetByUserResponse>.Ok(result.Value!)
                : HttpResult<GetByUserResponse>.NotFound(result.Error!.Description);
        }

        [HttpGet("search")]
        public async Task<HttpResult<GetByFilterResult>> Find([FromQuery] TaskLabelFilterDTO filter, CancellationToken cancellationToken)
        {
            var query = new GetByFilterQuery(filter);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<GetByFilterResult>.Ok(result.Value!)
                : HttpResult<GetByFilterResult>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllTaskLabelsResult>> GetAll([FromQuery] GetAllTaskLabelsQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<GetAllTaskLabelsResult>.Ok(result.Value!)
                : HttpResult<GetAllTaskLabelsResult>.NotFound(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<CreateTaskLabelResult>> Create([FromBody] CreateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<CreateTaskLabelResult>.Created(result.Value!)
                : HttpResult<CreateTaskLabelResult>.BadRequest(result.Error!.Description);
        }

        [HttpPost("batch")]
        public async Task<HttpResult<int>> CreateBatch([FromBody] IEnumerable<CreateTaskLabelCommand> commands, CancellationToken cancellationToken)
        {
            var tasks = commands.Select(command => _sender.Send(command, cancellationToken));
            var results = await Task.WhenAll(tasks);
            var failedResults = results.Where(r => !r.IsSuccess).ToList();

            return !failedResults.Any()
                ? HttpResult<int>.Created(results.Length)
                : HttpResult<int>.BadRequest(failedResults.Select(r => r.Error!.Description).ToArray());
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateTaskLabelResult>> Update(int id, [FromBody] UpdateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            var updateCommand = new UpdateTaskLabelCommand(id, command.Name, command.LabelColor);
            var result = await _sender.Send(updateCommand, cancellationToken);

            return result.IsSuccess
                ? HttpResult<UpdateTaskLabelResult>.Updated()
                : HttpResult<UpdateTaskLabelResult>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteTaskLabelResult>> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteTaskLabelCommand(id);
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<DeleteTaskLabelResult>.Deleted()
                : HttpResult<DeleteTaskLabelResult>.NotFound(result.Error!.Description);
        }
    }
}
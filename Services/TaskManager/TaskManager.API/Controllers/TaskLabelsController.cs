using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.Features.TaskLabels.Commands.Create;
using TaskManager.Application.Features.TaskLabels.Commands.Delete;
using TaskManager.Application.Features.TaskLabels.Commands.Update;
using TaskManager.Application.Features.TaskLabels.Queries.GetAll;
using TaskManager.Application.Features.TaskLabels.Queries.GetByFilter;
using TaskManager.Application.Features.TaskLabels.Queries.GetById;
using TaskManager.Application.Features.TaskLabels.Queries.GetByUser;

namespace TaskManager.API.Controllers
{
    [Route("api/task-labels")]
    public class TaskLabelsController : ApiController
    {
        private readonly ISender _sender;

        public TaskLabelsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetTaskLabelByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<object>> GetByUserId(int userId, CancellationToken cancellationToken)
        {
            var query = new GetByUserQuery(userId);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("search")]
        public async Task<HttpResult<object>> Find([FromQuery] TaskLabelFilterDTO filter, CancellationToken cancellationToken)
        {
            var query = new GetByFilterQuery(filter);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value?.TaskLabels!, result.Value?.Pagination!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll([FromQuery] GetAllTaskLabelsQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create([FromBody] CreateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost("batch")]
        public async Task<HttpResult<object>> CreateBatch([FromBody] IEnumerable<CreateTaskLabelCommand> commands, CancellationToken cancellationToken)
        {
            var tasks = commands.Select(command => _sender.Send(command, cancellationToken));
            var results = await Task.WhenAll(tasks);
            var failedResults = results.Where(r => !r.IsSuccess).ToList();

            return !failedResults.Any()
                ? HttpResult<object>.Created(results.Length)
                : HttpResult<object>.BadRequest(failedResults.Select(r => r.Error!.Description).ToArray());
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update(int id, [FromBody] UpdateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            var updateCommand = new UpdateTaskLabelCommand(id, command.Name, command.LabelColor);
            var result = await _sender.Send(updateCommand, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteTaskLabelCommand(id);
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.NotFound(result.Error!.Description);
        }
    }
}
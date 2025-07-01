using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.Features.TaskItems.Commands.CreateTaskItem;
using TaskManager.Application.Features.TaskItems.Commands.DeleteTaskItem;
using TaskManager.Application.Features.TaskItems.Commands.UpdateTaskItem;
using TaskManager.Application.Features.TaskItems.Queries.GetAll;
using TaskManager.Application.Features.TaskItems.Queries.GetByFilter;
using TaskManager.Application.Features.TaskItems.Queries.GetById;

namespace TaskManager.API.Controllers
{
    [Route("api/v1/task-items")]
    public class TaskItemsController : ApiController
    {
        private readonly ISender _sender;

        public TaskItemsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetTaskItemByIdResult>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetTaskItemByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<GetTaskItemByIdResult>.Ok(result.Value!)
                : HttpResult<GetTaskItemByIdResult>.NotFound(result.Error!);
        }

        [HttpGet("search")]
        public async Task<HttpResult<GetByFilterResult>> Find([FromQuery] TaskItemFilterDTO filter, CancellationToken cancellationToken)
        {
            var query = new GetByFilterQuery(filter);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<GetByFilterResult>.Ok(result.Value!)
                : HttpResult<GetByFilterResult>.NotFound(result.Error!);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllTaskItemsResult>> GetAll([FromQuery] GetAllTaskItemsQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<GetAllTaskItemsResult>.Ok(result.Value!)
                : HttpResult<GetAllTaskItemsResult>.NotFound(result.Error!);
        }

        [HttpPost]
        public async Task<HttpResult<CreateTaskItemResult>> Create([FromBody] CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<CreateTaskItemResult>.Created(result.Value!)
                : HttpResult<CreateTaskItemResult>.BadRequest(result.Error!);
        }

        [HttpPost("batch")]
        public async Task<HttpResult<int>> CreateBatch([FromBody] IEnumerable<CreateTaskItemCommand> commands, CancellationToken cancellationToken)
        {
            if (commands == null || !commands.Any())
                return HttpResult<int>.BadRequest("A lista de tarefas não pode ser vazia");

            var tasks = commands.Select(command => _sender.Send(command, cancellationToken));
            var results = await Task.WhenAll(tasks);

            var failedResults = results.Where(r => !r.IsSuccess).ToList();

            return !failedResults.Any()
                ? HttpResult<int>.Created(results.Length)
                : HttpResult<int>.BadRequest(failedResults.Select(r => r.Error!).ToArray());
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateTaskItemCommandResult>> Update(int id, [FromBody] UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            var updateCommand = new UpdateTaskItemCommand(id, command.Title, command.Description, command.Status, command.Priority, command.DueDate);
            var result = await _sender.Send(updateCommand, cancellationToken);

            return result.IsSuccess
                ? HttpResult<UpdateTaskItemCommandResult>.Updated()
                : HttpResult<UpdateTaskItemCommandResult>.BadRequest(result.Error!);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteTaskItemResult>> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteTaskItemCommand(id);
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<DeleteTaskItemResult>.Deleted()
                : HttpResult<DeleteTaskItemResult>.NotFound(result.Error!);
        }
    }
}
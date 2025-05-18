using BuildingBlocks.Results;
using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Application.TaskItems.Commands.CreateTaskItem;
using TaskManager.Application.TaskItems.Commands.DeleteTaskItem;
using TaskManager.Application.TaskItems.Commands.UpdateTaskItem;
using TaskManager.Application.TaskItems.Queries.GetAll;
using TaskManager.Application.TaskItems.Queries.GetByFilter;
using TaskManager.Application.TaskItems.Queries.GetById;

namespace TaskManager.API.Controllers
{
    [Route("api/v1/task-items")]
    public class TaskItemsController : ApiController
    {
        private readonly IMediator _mediator;

        public TaskItemsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetTaskItemByIdResult>> GetById(int id, CancellationToken cancellationToken)
        {
            GetTaskItemByIdQuery query = new(id);
            GetTaskItemByIdResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetTaskItemByIdResult>.Ok(result);
        }

        [HttpGet("search")]
        public async Task<HttpResult<GetByFilterResult>> Find([FromQuery] TaskItemFilterDTO filter, CancellationToken cancellationToken)
        {
            GetByFilterQuery query = new(filter);
            GetByFilterResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByFilterResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllTaskItemsResult>> GetAll([FromQuery] GetAllTaskItemsQuery query, CancellationToken cancellationToken)
        {
            GetAllTaskItemsResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetAllTaskItemsResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateTaskItemResult>> Create([FromBody] CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
            CreateTaskItemResult result = await _mediator.Send(command, cancellationToken);
            return HttpResult<CreateTaskItemResult>.Created(result);
        }

        [HttpPost("batch")]
        public async Task<HttpResult<int>> CreateBatch([FromBody] IEnumerable<CreateTaskItemCommand> commands, CancellationToken cancellationToken)
        {
            var tasks = commands.Select(command => _mediator.Send(command, cancellationToken));
            var results = await Task.WhenAll(tasks);
            int createdCount = results.Length;
            return HttpResult<int>.Created(createdCount);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateTaskItemCommandResult>> Update(int id, [FromBody] UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            UpdateTaskItemCommand updateTaskItemCommand = new(id, command.Title, command.Description, command.Status, command.Priority, command.DueDate);
            UpdateTaskItemCommandResult result = await _mediator.Send(updateTaskItemCommand, cancellationToken);
            return HttpResult<UpdateTaskItemCommandResult>.Updated();
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteTaskItemResult>> Delete(int id, CancellationToken cancellationToken)
        {
            DeleteTaskItemCommand command = new(id);
            DeleteTaskItemResult result = await _mediator.Send(command, cancellationToken);
            return HttpResult<DeleteTaskItemResult>.Deleted();
        }
    }
}
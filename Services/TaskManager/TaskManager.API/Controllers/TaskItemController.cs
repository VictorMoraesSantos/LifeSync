using BuildingBlocks.Results;
using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.TaskItems.Commands.CreateTaskItem;
using TaskManager.Application.TaskItems.Commands.DeleteTaskItem;
using TaskManager.Application.TaskItems.Commands.UpdateTaskItem;
using TaskManager.Application.TaskItems.Queries.GetAll;
using TaskManager.Application.TaskItems.Queries.GetAllByUserId;
using TaskManager.Application.TaskItems.Queries.GetByDueDate;
using TaskManager.Application.TaskItems.Queries.GetById;
using TaskManager.Application.TaskItems.Queries.GetByLabel;
using TaskManager.Application.TaskItems.Queries.GetByPriority;
using TaskManager.Application.TaskItems.Queries.GetByStatus;
using TaskManager.Application.TaskItems.Queries.GetByTitle;

namespace TaskManager.API.Controllers
{
    public class TaskItemController : ApiController
    {
        private readonly IMediator _mediator;

        public TaskItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("id")]
        public async Task<HttpResult<GetByIdResult>> GetById([FromQuery] GetByIdQuery query, CancellationToken cancellationToken)
        {
            GetByIdResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByIdResult>.Ok(result);
        }

        [HttpGet("all")]
        public async Task<HttpResult<GetAllTaskItemsResult>> GetAll([FromQuery] GetAllTaskItemsQuery query, CancellationToken cancellationToken)
        {
            GetAllTaskItemsResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetAllTaskItemsResult>.Ok(result);
        }

        [HttpGet("user-id")]
        public async Task<HttpResult<GetAllByUserIdResult>> GetAllByUserId([FromQuery] GetAllByUserIdQuery query, CancellationToken cancellationToken)
        {
            GetAllByUserIdResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetAllByUserIdResult>.Ok(result);
        }

        [HttpGet("due-date")]
        public async Task<HttpResult<GetByDueDateResult>> GetByDueDate([FromQuery] GetByDueDateQuery query, CancellationToken cancellationToken)
        {
            GetByDueDateResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByDueDateResult>.Ok(result);
        }

        [HttpGet("label")]
        public async Task<HttpResult<GetByLabelResult>> GetByLabel([FromQuery] GetByLabelQuery query, CancellationToken cancellationToken)
        {
            GetByLabelResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByLabelResult>.Ok(result);
        }

        [HttpGet("priority")]
        public async Task<HttpResult<GetByPriorityResult>> GetByPriority([FromQuery] GetByPriorityQuery query, CancellationToken cancellationToken)
        {
            GetByPriorityResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByPriorityResult>.Ok(result);
        }

        [HttpGet("status")]
        public async Task<HttpResult<GetByStatusResult>> GetByStatus([FromQuery] GetByStatusQuery query, CancellationToken cancellationToken)
        {
            GetByStatusResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByStatusResult>.Ok(result);
        }

        [HttpGet("title")]
        public async Task<HttpResult<GetByTitleResult>> GetByTitle([FromQuery] GetByTitleQuery query, CancellationToken cancellationToken)
        {
            GetByTitleResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByTitleResult>.Ok(result);
        }

        [HttpPost("create")]
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

        [HttpPut("update")]
        public async Task<HttpResult<UpdateTaskItemCommandResult>> Update([FromBody] UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            UpdateTaskItemCommandResult result = await _mediator.Send(command, cancellationToken);
            return HttpResult<UpdateTaskItemCommandResult>.Updated();
        }

        [HttpDelete("delete")]
        public async Task<HttpResult<DeleteTaskItemResult>> Delete([FromBody] DeleteTaskItemCommand command, CancellationToken cancellationToken)
        {
            DeleteTaskItemResult result = await _mediator.Send(command, cancellationToken);
            return HttpResult<DeleteTaskItemResult>.Deleted();
        }

        [HttpGet("search")]
        public async Task<HttpResult<IEnumerable<TaskItemDTO>>> Find([FromQuery] string property, [FromQuery] string value, CancellationToken cancellationToken)
        {
            return HttpResult<IEnumerable<TaskItemDTO>>.BadRequest("Busca dinâmica ainda não implementada.");
        }
    }
}
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
        public async Task<ActionResult<HttpResult<TaskItemDTO>>> GetById([FromQuery] GetByIdQuery query, CancellationToken cancellationToken)
        {
            TaskItemDTO result = await _mediator.Send(query, cancellationToken);
            return HttpResult<TaskItemDTO>.Ok(result);
        }

        [HttpGet("all")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskItemDTO>>>> GetAll([FromQuery] GetAllTaskItemsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItemDTO> result = await _mediator.Send(query, cancellationToken);
            return HttpResult<IEnumerable<TaskItemDTO>>.Ok(result);
        }

        [HttpGet("user-id")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskItemDTO>>>> GetAllByUserId([FromQuery] GetAllByUserIdQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItemDTO> result = await _mediator.Send(query, cancellationToken);
            return HttpResult<IEnumerable<TaskItemDTO>>.Ok(result);
        }

        [HttpGet("due-date")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskItemDTO>>>> GetByDueDate([FromQuery] GetByDueDateQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItemDTO> result = await _mediator.Send(query, cancellationToken);
            return HttpResult<IEnumerable<TaskItemDTO>>.Ok(result);
        }

        [HttpGet("label")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskItemDTO>>>> GetByLabel([FromQuery] GetByLabelQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItemDTO> result = await _mediator.Send(query, cancellationToken);
            return HttpResult<IEnumerable<TaskItemDTO>>.Ok(result);
        }

        [HttpGet("priority")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskItemDTO>>>> GetByPriority([FromQuery] GetByPriorityQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItemDTO> result = await _mediator.Send(query, cancellationToken);
            return HttpResult<IEnumerable<TaskItemDTO>>.Ok(result);
        }

        [HttpGet("status")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskItemDTO>>>> GetByStatus([FromQuery] GetByStatusQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItemDTO> result = await _mediator.Send(query, cancellationToken);
            return HttpResult<IEnumerable<TaskItemDTO>>.Ok(result);
        }

        [HttpGet("title")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskItemDTO>>>> GetByTitle([FromQuery] GetByTitleQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskItemDTO> result = await _mediator.Send(query, cancellationToken);
            return HttpResult<IEnumerable<TaskItemDTO>>.Ok(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult<HttpResult<int>>> Create([FromBody] CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
            int result = await _mediator.Send(command, cancellationToken);
            return HttpResult<int>.Created(result);
        }

        [HttpPost("batch")]
        public async Task<ActionResult<HttpResult<int>>> CreateBatch([FromBody] IEnumerable<CreateTaskItemCommand> commands, CancellationToken cancellationToken)
        {
            var tasks = commands.Select(command => _mediator.Send(command, cancellationToken));
            var results = await Task.WhenAll(tasks);
            int createdCount = results.Length;
            return HttpResult<int>.Created(createdCount);
        }

        [HttpPut("update")]
        public async Task<ActionResult<HttpResult<bool>>> Update([FromBody] UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            bool result = await _mediator.Send(command, cancellationToken);
            return HttpResult<bool>.Updated(result);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<HttpResult<bool>>> Delete([FromBody] DeleteTaskItemCommand command, CancellationToken cancellationToken)
        {
            bool result = await _mediator.Send(command, cancellationToken);
            return HttpResult<bool>.Deleted(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskItemDTO>>>> Find([FromQuery] string property, [FromQuery] string value, CancellationToken cancellationToken)
        {
            return HttpResult<IEnumerable<TaskItemDTO>>.BadRequest("Busca dinâmica ainda não implementada.");
        }
    }
}
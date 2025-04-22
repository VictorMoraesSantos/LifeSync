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
        public async Task<HttpResult<GetByIdResponse>> GetById([FromQuery] GetByIdQuery query, CancellationToken cancellationToken)
        {
            GetByIdResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByIdResponse>.Ok(result);
        }

        [HttpGet("all")]
        public async Task<HttpResult<GetAllTaskItemsResponse>> GetAll([FromQuery] GetAllTaskItemsQuery query, CancellationToken cancellationToken)
        {
            GetAllTaskItemsResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetAllTaskItemsResponse>.Ok(result);
        }

        [HttpGet("user-id")]
        public async Task<HttpResult<GetAllByUserIdResponse>> GetAllByUserId([FromQuery] GetAllByUserIdQuery query, CancellationToken cancellationToken)
        {
            GetAllByUserIdResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetAllByUserIdResponse>.Ok(result);
        }

        [HttpGet("due-date")]
        public async Task<HttpResult<GetByDueDateResponse>> GetByDueDate([FromQuery] GetByDueDateQuery query, CancellationToken cancellationToken)
        {
            GetByDueDateResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByDueDateResponse>.Ok(result);
        }

        [HttpGet("label")]
        public async Task<HttpResult<GetByLabelResponse>> GetByLabel([FromQuery] GetByLabelQuery query, CancellationToken cancellationToken)
        {
            GetByLabelResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByLabelResponse>.Ok(result);
        }

        [HttpGet("priority")]
        public async Task<HttpResult<GetByPriorityResponse>> GetByPriority([FromQuery] GetByPriorityQuery query, CancellationToken cancellationToken)
        {
            GetByPriorityResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByPriorityResponse>.Ok(result);
        }

        [HttpGet("status")]
        public async Task<HttpResult<GetByStatusResponse>> GetByStatus([FromQuery] GetByStatusQuery query, CancellationToken cancellationToken)
        {
            GetByStatusResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByStatusResponse>.Ok(result);
        }

        [HttpGet("title")]
        public async Task<HttpResult<GetByTitleResponse>> GetByTitle([FromQuery] GetByTitleQuery query, CancellationToken cancellationToken)
        {
            GetByTitleResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByTitleResponse>.Ok(result);
        }

        [HttpPost("create")]
        public async Task<HttpResult<CreateTaskItemResponse>> Create([FromBody] CreateTaskItemCommand command, CancellationToken cancellationToken)
        {
            CreateTaskItemResponse result = await _mediator.Send(command, cancellationToken);
            return HttpResult<CreateTaskItemResponse>.Created(result);
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
        public async Task<HttpResult<UpdateTaskItemCommandResponse>> Update([FromBody] UpdateTaskItemCommand command, CancellationToken cancellationToken)
        {
            UpdateTaskItemCommandResponse result = await _mediator.Send(command, cancellationToken);
            return HttpResult<UpdateTaskItemCommandResponse>.Updated(result);
        }

        [HttpDelete("delete")]
        public async Task<HttpResult<DeleteTaskItemResponse>> Delete([FromBody] DeleteTaskItemCommand command, CancellationToken cancellationToken)
        {
            DeleteTaskItemResponse result = await _mediator.Send(command, cancellationToken);
            return HttpResult<DeleteTaskItemResponse>.Deleted(result);
        }

        [HttpGet("search")]
        public async Task<HttpResult<IEnumerable<TaskItemDTO>>> Find([FromQuery] string property, [FromQuery] string value, CancellationToken cancellationToken)
        {
            return HttpResult<IEnumerable<TaskItemDTO>>.BadRequest("Busca dinâmica ainda não implementada.");
        }
    }
}
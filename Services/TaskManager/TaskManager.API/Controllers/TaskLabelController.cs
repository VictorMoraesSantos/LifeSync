using BuildingBlocks.Results;
using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.TaskLabels.Commands.CreateTaskLabel;
using TaskManager.Application.TaskLabels.Commands.DeleteTaskLabel;
using TaskManager.Application.TaskLabels.Commands.UpdateTaskLabel;
using TaskManager.Application.TaskLabels.Queries.GetAll;
using TaskManager.Application.TaskLabels.Queries.GetById;
using TaskManager.Application.TaskLabels.Queries.GetByName;
using TaskManager.Application.TaskLabels.Queries.GetByUserId;

namespace TaskManager.API.Controllers
{
    public class TaskLabelController : ApiController
    {
        private readonly IMediator _mediator;

        public TaskLabelController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("id")]
        public async Task<HttpResult<GetTaskLabelByIdResponse>> GetById([FromQuery] GetTaskLabelByIdQuery query, CancellationToken cancellationToken)
        {
            GetTaskLabelByIdResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetTaskLabelByIdResponse>.Ok(result);
        }

        [HttpGet("all")]
        public async Task<HttpResult<GetAllTaskLabelsResponse>> GetAll([FromQuery] GetAllTaskLabelsQuery query, CancellationToken cancellationToken)
        {
            GetAllTaskLabelsResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetAllTaskLabelsResponse>.Ok(result);
        }

        [HttpGet("user-id")]
        public async Task<HttpResult<GetTaskLabelByUserIdResponse>> GetAllByUserId([FromQuery] GetTaskLabelByUserIdQuery query, CancellationToken cancellationToken)
        {
            GetTaskLabelByUserIdResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetTaskLabelByUserIdResponse>.Ok(result);
        }

        [HttpGet("name")]
        public async Task<HttpResult<GetTaskLabelByNameResponse>> GetByName([FromQuery] GetTaskLabelByNameQuery query, CancellationToken cancellationToken)
        {
            GetTaskLabelByNameResponse result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetTaskLabelByNameResponse>.Ok(result);
        }

        [HttpPost("create")]
        public async Task<HttpResult<CreateTaskLabelResponse>> Create([FromBody] CreateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            CreateTaskLabelResponse result = await _mediator.Send(command, cancellationToken);
            return HttpResult<CreateTaskLabelResponse>.Created(result);
        }

        [HttpPost("batch")]
        public async Task<HttpResult<int>> CreateBatch([FromBody] IEnumerable<CreateTaskLabelCommand> commands, CancellationToken cancellationToken)
        {
            var tasks = commands.Select(command => _mediator.Send(command, cancellationToken));
            var results = await Task.WhenAll(tasks);
            int createdCount = results.Length;
            return HttpResult<int>.Created(createdCount);
        }

        [HttpPut("update")]
        public async Task<HttpResult<UpdateTaskLabelResponse>> Update([FromBody] UpdateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            UpdateTaskLabelResponse result = await _mediator.Send(command, cancellationToken);
            return HttpResult<UpdateTaskLabelResponse>.Updated(result);
        }

        [HttpDelete("delete")]
        public async Task<HttpResult<DeleteTaskLabelResponse>> Delete([FromBody] DeleteTaskLabelCommand command, CancellationToken cancellationToken)
        {
            DeleteTaskLabelResponse result = await _mediator.Send(command, cancellationToken);
            return HttpResult<DeleteTaskLabelResponse>.Deleted(result);
        }

        [HttpGet("search")]
        public async Task<HttpResult<IEnumerable<TaskLabelDTO>>> Find([FromQuery] string property, [FromQuery] string value, CancellationToken cancellationToken)
        {
            return HttpResult<IEnumerable<TaskLabelDTO>>.BadRequest("Busca dinâmica ainda não implementada.");
        }
    }
}

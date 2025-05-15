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
        public async Task<HttpResult<GetTaskLabelByIdResult>> GetById([FromQuery] GetTaskLabelByIdQuery query, CancellationToken cancellationToken)
        {
            GetTaskLabelByIdResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetTaskLabelByIdResult>.Ok(result);
        }

        [HttpGet("all")]
        public async Task<HttpResult<GetAllTaskLabelsResult>> GetAll([FromQuery] GetAllTaskLabelsQuery query, CancellationToken cancellationToken)
        {
            GetAllTaskLabelsResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetAllTaskLabelsResult>.Ok(result);
        }

        [HttpGet("user-id")]
        public async Task<HttpResult<GetTaskLabelByUserIdResult>> GetAllByUserId([FromQuery] GetTaskLabelByUserIdQuery query, CancellationToken cancellationToken)
        {
            GetTaskLabelByUserIdResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetTaskLabelByUserIdResult>.Ok(result);
        }

        [HttpGet("name")]
        public async Task<HttpResult<GetTaskLabelByNameResult>> GetByName([FromQuery] GetTaskLabelByNameQuery query, CancellationToken cancellationToken)
        {
            GetTaskLabelByNameResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetTaskLabelByNameResult>.Ok(result);
        }

        [HttpPost("create")]
        public async Task<HttpResult<CreateTaskLabelResult>> Create([FromBody] CreateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            CreateTaskLabelResult result = await _mediator.Send(command, cancellationToken);
            return HttpResult<CreateTaskLabelResult>.Created(result);
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
        public async Task<HttpResult<UpdateTaskLabelResult>> Update([FromBody] UpdateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            UpdateTaskLabelResult result = await _mediator.Send(command, cancellationToken);
            return HttpResult<UpdateTaskLabelResult>.Updated();
        }

        [HttpDelete("delete")]
        public async Task<HttpResult<DeleteTaskLabelResult>> Delete([FromBody] DeleteTaskLabelCommand command, CancellationToken cancellationToken)
        {
            DeleteTaskLabelResult result = await _mediator.Send(command, cancellationToken);
            return HttpResult<DeleteTaskLabelResult>.Deleted();
        }

        [HttpGet("search")]
        public async Task<HttpResult<IEnumerable<TaskLabelDTO>>> Find([FromQuery] string property, [FromQuery] string value, CancellationToken cancellationToken)
        {
            return HttpResult<IEnumerable<TaskLabelDTO>>.BadRequest("Busca dinâmica ainda não implementada.");
        }
    }
}

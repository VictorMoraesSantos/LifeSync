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
        public async Task<ActionResult<HttpResult<TaskLabelDTO?>>> GetById([FromQuery] GetByIdQuery query, CancellationToken cancellationToken)
        {
            TaskLabelDTO? result = await _mediator.Send(query, cancellationToken);
            return HttpResult<TaskLabelDTO?>.Ok(result);
        }

        [HttpGet("all")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskLabelDTO>>>> GetAll([FromQuery] GetAllTaskLabelsQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabelDTO> result = await _mediator.Send(query, cancellationToken);
            return HttpResult<IEnumerable<TaskLabelDTO>>.Ok(result);
        }

        [HttpGet("user-id")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskLabelDTO?>>>> GetAllByUserId([FromQuery] GetByUserIdQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabelDTO>? result = await _mediator.Send(query, cancellationToken);
            return HttpResult<IEnumerable<TaskLabelDTO?>>.Ok(result);
        }

        [HttpGet("name")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskLabelDTO?>>>> GetByName([FromQuery] GetByNameQuery query, CancellationToken cancellationToken)
        {
            IEnumerable<TaskLabelDTO>? result = await _mediator.Send(query, cancellationToken);
            return HttpResult<IEnumerable<TaskLabelDTO?>>.Ok(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult<HttpResult<int>>> Create([FromBody] CreateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            int result = await _mediator.Send(command, cancellationToken);
            return HttpResult<int>.Created(result);
        }

        [HttpPost("batch")]
        public async Task<ActionResult<HttpResult<int>>> CreateBatch([FromBody] IEnumerable<CreateTaskLabelCommand> commands, CancellationToken cancellationToken)
        {
            var tasks = commands.Select(command => _mediator.Send(command, cancellationToken));
            var results = await Task.WhenAll(tasks);
            int createdCount = results.Length;
            return HttpResult<int>.Created(createdCount);
        }

        [HttpPut("update")]
        public async Task<ActionResult<HttpResult<bool>>> Update([FromBody] UpdateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            bool result = await _mediator.Send(command, cancellationToken);
            return HttpResult<bool>.Updated(result);
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<HttpResult<bool>>> Delete([FromBody] DeleteTaskLabelCommand command, CancellationToken cancellationToken)
        {
            bool result = await _mediator.Send(command, cancellationToken);
            return HttpResult<bool>.Deleted(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<HttpResult<IEnumerable<TaskLabelDTO>>>> Find([FromQuery] string property, [FromQuery] string value, CancellationToken cancellationToken)
        {
            return HttpResult<IEnumerable<TaskLabelDTO>>.BadRequest("Busca dinâmica ainda não implementada.");
        }
    }
}

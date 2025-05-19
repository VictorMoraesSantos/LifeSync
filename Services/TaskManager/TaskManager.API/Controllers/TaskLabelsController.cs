using BuildingBlocks.Results;
using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs.Filters;
using TaskManager.Application.TaskLabels.Commands.CreateTaskLabel;
using TaskManager.Application.TaskLabels.Commands.DeleteTaskLabel;
using TaskManager.Application.TaskLabels.Commands.UpdateTaskLabel;
using TaskManager.Application.TaskLabels.Queries.GetAll;
using TaskManager.Application.TaskLabels.Queries.GetByFilter;
using TaskManager.Application.TaskLabels.Queries.GetById;

namespace TaskManager.API.Controllers
{
    [Route("api/v1/task-labels")]
    public class TaskLabelsController : ApiController
    {
        private readonly IMediator _mediator;

        public TaskLabelsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetTaskLabelByIdResult>> GetById(int id, CancellationToken cancellationToken)
        {
            GetTaskLabelByIdQuery query = new(id);
            GetTaskLabelByIdResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetTaskLabelByIdResult>.Ok(result);
        }

        [HttpGet("search")]
        public async Task<HttpResult<GetByFilterResult>> Find([FromQuery] TaskLabelFilterDTO filter, CancellationToken cancellationToken)
        {
            GetByFilterQuery query = new(filter);
            GetByFilterResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetByFilterResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllTaskLabelsResult>> GetAll([FromQuery] GetAllTaskLabelsQuery query, CancellationToken cancellationToken)
        {
            GetAllTaskLabelsResult result = await _mediator.Send(query, cancellationToken);
            return HttpResult<GetAllTaskLabelsResult>.Ok(result);
        }

        [HttpPost]
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

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateTaskLabelResult>> Update(int id, [FromBody] UpdateTaskLabelCommand command, CancellationToken cancellationToken)
        {
            UpdateTaskLabelCommand updateCommand = new(id, command.Name, command.LabelColor);
            UpdateTaskLabelResult result = await _mediator.Send(updateCommand, cancellationToken);
            return HttpResult<UpdateTaskLabelResult>.Updated();
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteTaskLabelResult>> Delete(int id, CancellationToken cancellationToken)
        {
            DeleteTaskLabelCommand command = new(id);
            DeleteTaskLabelResult result = await _mediator.Send(command, cancellationToken);
            return HttpResult<DeleteTaskLabelResult>.Deleted();
        }
    }
}

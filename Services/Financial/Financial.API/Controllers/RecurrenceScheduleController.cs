using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Financial.Application.DTOs.RecurrenceSchedule;
using Financial.Application.Features.RecurrenceSchedules.Commands.Deactivate;
using Financial.Application.Features.RecurrenceSchedules.Commands.Delete;
using Financial.Application.Features.RecurrenceSchedules.Commands.Update;
using Financial.Application.Features.RecurrenceSchedules.Queries.GetByFilter;
using Financial.Application.Features.RecurrenceSchedules.Queries.GetById;
using Financial.Application.Features.RecurrenceSchedules.Queries.GetByUserId;
using Microsoft.AspNetCore.Mvc;

namespace Financial.API.Controllers
{
    public class RecurrenceScheduleController : ApiController
    {
        private readonly ISender _sender;

        public RecurrenceScheduleController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id}")]
        public async Task<HttpResult<object>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetRecurrenceScheduleByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("user/{userId}")]
        public async Task<HttpResult<object>> GetByUserId(int userId, CancellationToken cancellationToken)
        {
            var query = new GetRecurrenceScheduleByUserIdQuery(userId);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("search")]
        public async Task<HttpResult<object>> Search([FromQuery] RecurrenceScheduleFilterDTO filter, CancellationToken cancellationToken)
        {
            var query = new GetRecurrenceScheduleByFilterQuery(filter);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value.Items, result.Value.Pagination)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update(int id, [FromBody] UpdateRecurrenceScheduleCommand command, CancellationToken cancellationToken)
        {
            var updatedCommand = new UpdateRecurrenceScheduleCommand(id, command.Frequency, command.EndDate, command.MaxOccurrences);
            var result = await _sender.Send(updatedCommand, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPatch("{id:int}/deactivate")]
        public async Task<HttpResult<object>> DeactivateAsync(
            int id, CancellationToken cancellationToken)
        {
            var command = new DeactivateRecurrenceScheduleCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.IsSuccess)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteRecurrenceScheduleCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.NotFound(result.Error!.Description);
        }
    }
}

using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Features.DailyProgress.Commands.Create;
using Nutrition.Application.Features.DailyProgress.Commands.Delete;
using Nutrition.Application.Features.DailyProgress.Commands.SetGoal;
using Nutrition.Application.Features.DailyProgress.Commands.Update;
using Nutrition.Application.Features.DailyProgress.Queries.GetAll;
using Nutrition.Application.Features.DailyProgress.Queries.GetById;
using Nutrition.Application.Features.DailyProgress.Queries.GetByUser;

namespace Nutrition.API.Controllers
{
    [Route("api/daily-progresses")]
    public class DailyProgressesController : ApiController
    {
        private readonly ISender _sender;

        public DailyProgressesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> Get(int id)
        {
            var query = new GetDailyProgressQuery(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<object>> GetByUserId(int userId)
        {
            GetAllDailyProgressesByUserIdQuery query = new(userId);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll()
        {
            GetDailyProgressesQuery query = new();
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create([FromBody] CreateDailyProgressCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update(int id, [FromBody] UpdateDailyProgressCommand command)
        {
            UpdateDailyProgressCommand updateCommand = new(
                id,
                command.CaloriesConsumed,
                command.LiquidsConsumedMl,
                command.Goal);

            var result = await _sender.Send(updateCommand);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description)
                    : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id)
        {
            DeleteDailyProgressCommand command = new(id);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpPost("{id:int}/set-goal")]
        public async Task<HttpResult<object>> SetConsumed(int id, [FromBody] SetGoalCommand command)
        {
            DailyGoalDTO goal = command.Goal;
            SetGoalCommand setConsumedCommand = new(id, goal);
            var result = await _sender.Send(setConsumedCommand);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description!)
                    : HttpResult<object>.BadRequest(result.Error!.Description);
        }
    }
}
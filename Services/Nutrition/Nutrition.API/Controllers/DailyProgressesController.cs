using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.DTOs.DailyProgress;
using Nutrition.Application.Features.DailyProgress.Commands.Create;
using Nutrition.Application.Features.DailyProgress.Commands.Delete;
using Nutrition.Application.Features.DailyProgress.Commands.SetGoal;
using Nutrition.Application.Features.DailyProgress.Commands.Update;
using Nutrition.Application.Features.DailyProgress.Queries.Get;
using Nutrition.Application.Features.DailyProgress.Queries.GetAll;
using Nutrition.Application.Features.DailyProgress.Queries.GetByUser;

namespace Nutrition.API.Controllers
{
    [Route("api/v1/daily-progresses")]
    public class DailyProgressesController : ApiController
    {
        private readonly ISender _sender;

        public DailyProgressesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetDailyProgressResult>> Get(int id)
        {
            var query = new GetDailyProgressQuery(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetDailyProgressResult>.Ok(result.Value!)
                : HttpResult<GetDailyProgressResult>.NotFound(result.Error!);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<GetAllDailyProgressesByUserIdResult>> GetByUserId(int userId)
        {
            GetAllDailyProgressesByUserIdQuery query = new(userId);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetAllDailyProgressesByUserIdResult>.Ok(result.Value!)
                : HttpResult<GetAllDailyProgressesByUserIdResult>.NotFound(result.Error!);
        }

        [HttpGet]
        public async Task<HttpResult<GetDailyProgressesResult>> GetAll()
        {
            GetDailyProgressesQuery query = new();
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetDailyProgressesResult>.Ok(result.Value!)
                : HttpResult<GetDailyProgressesResult>.BadRequest(result.Error!);
        }

        [HttpPost]
        public async Task<HttpResult<CreateDailyProgressResult>> Create([FromBody] CreateDailyProgressCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<CreateDailyProgressResult>.Created(result.Value!)
                : HttpResult<CreateDailyProgressResult>.BadRequest(result.Error!);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateDailyProgressResult>> Update(int id, [FromBody] UpdateDailyProgressCommand command)
        {
            UpdateDailyProgressCommand updateCommand = new(
                id,
                command.CaloriesConsumed,
                command.LiquidsConsumedMl,
                command.Goal);

            var result = await _sender.Send(updateCommand);

            return result.IsSuccess
                ? HttpResult<UpdateDailyProgressResult>.Ok(result.Value!)
                : result.Error.Contains("NotFound")
                    ? HttpResult<UpdateDailyProgressResult>.NotFound(result.Error!)
                    : HttpResult<UpdateDailyProgressResult>.BadRequest(result.Error!);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteDailyProgressResult>> Delete(int id)
        {
            DeleteDailyProgressCommand command = new(id);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<DeleteDailyProgressResult>.Deleted()
                : HttpResult<DeleteDailyProgressResult>.NotFound(result.Error!);
        }

        [HttpPost("{id:int}/set-goal")]
        public async Task<HttpResult<SetGoalResult>> SetConsumed(int id, [FromBody] SetGoalCommand command)
        {
            DailyGoalDTO goal = command.Goal;
            SetGoalCommand setConsumedCommand = new(id, goal);
            var result = await _sender.Send(setConsumedCommand);

            return result.IsSuccess
                ? HttpResult<SetGoalResult>.Ok(result.Value!)
                : result.Error.Contains("NotFound")
                    ? HttpResult<SetGoalResult>.NotFound(result.Error!)
                    : HttpResult<SetGoalResult>.BadRequest(result.Error!);
        }
    }
}
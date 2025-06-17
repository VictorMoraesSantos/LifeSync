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
    [Route("api/daily-progresses")]
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
            GetDailyProgressQuery query = new(id);
            GetDailyProgressResult result = await _sender.Send(query);
            return HttpResult<GetDailyProgressResult>.Ok(result);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<GetAllDailyProgressesByUserIdResult>> GetByUserId(int userId)
        {
            GetAllDailyProgressesByUserIdQuery query = new(userId);
            GetAllDailyProgressesByUserIdResult result = await _sender.Send(query);
            return HttpResult<GetAllDailyProgressesByUserIdResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetDailyProgressesResult>> GetAll()
        {
            GetDailyProgressesQuery query = new();
            GetDailyProgressesResult result = await _sender.Send(query);
            return HttpResult<GetDailyProgressesResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateDailyProgressResult>> Create([FromBody] CreateDailyProgressCommand command)
        {
            CreateDailyProgressResult result = await _sender.Send(command);
            return HttpResult<CreateDailyProgressResult>.Created(result);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateDailyProgressResult>> Update(int id, [FromBody] UpdateDailyProgressCommand command)
        {
            UpdateDailyProgressCommand updateCommand = new(
                id,
                command.CaloriesConsumed,
                command.LiquidsConsumedMl,
                command.Goal);
            UpdateDailyProgressResult result = await _sender.Send(updateCommand);
            return HttpResult<UpdateDailyProgressResult>.Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteDailyProgressResult>> Delete(int id)
        {
            DeleteDailyProgressCommand command = new(id);
            DeleteDailyProgressResult result = await _sender.Send(command);
            return HttpResult<DeleteDailyProgressResult>.Deleted();
        }

        [HttpPost("{id:int}/set-goal")]
        public async Task<HttpResult<SetGoalResult>> SetConsumed(int id, [FromBody] SetGoalCommand command)
        {
            DailyGoalDTO goal = command.Goal;
            SetGoalCommand setConsumedCommand = new(id, goal);
            SetGoalResult result = await _sender.Send(setConsumedCommand);
            return HttpResult<SetGoalResult>.Ok(result);
        }
    }
}

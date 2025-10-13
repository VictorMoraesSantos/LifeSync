using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Features.Meal.Commands.AddMealFood;
using Nutrition.Application.Features.Meal.Commands.Create;
using Nutrition.Application.Features.Meal.Commands.Delete;
using Nutrition.Application.Features.Meal.Commands.RemoveMealFood;
using Nutrition.Application.Features.Meal.Commands.Update;
using Nutrition.Application.Features.Meal.Queries.GetById;
using Nutrition.Application.Features.Meal.Queries.GetAll;
using Nutrition.Application.Features.Meal.Queries.GetByDiary;

namespace Nutrition.API.Controllers
{
    public class MealsController : ApiController
    {
        private readonly ISender _sender;

        public MealsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> Get(int id)
        {
            GetMealQuery query = new(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("diary/{id:int}")]
        public async Task<HttpResult<object>> GetByDiary(int id)
        {
            GetByDiaryQuery query = new(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error.Description!)
                    : HttpResult<object>.InternalError(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll([FromQuery] GetMealsQuery query)
        {
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.InternalError(result.Error!.Description);
        }

        [HttpPost("{mealId}/foods")]
        public async Task<HttpResult<object>> AddMealFood(int mealId, [FromBody] CreateMealFoodDTO dto, CancellationToken cancellationToken)
        {
            AddMealFoodCommand command = new(mealId, dto);
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description)
                    : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{mealId}/foods/{foodId}")]
        public async Task<HttpResult<object>> RemoveMealFood(int mealId, int foodId, CancellationToken cancellationToken)
        {
            RemoveMealFoodCommand command = new(mealId, foodId);
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description)
                    : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create([FromBody] CreateMealCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update(int id, [FromBody] UpdateMealCommand command)
        {
            UpdateMealCommand updateMealCommand = new(
                id,
                command.Name,
                command.Description);
            var result = await _sender.Send(updateMealCommand);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description!)
                    : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id)
        {
            DeleteMealCommand command = new(id);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.NotFound(result.Error!.Description);
        }
    }
}
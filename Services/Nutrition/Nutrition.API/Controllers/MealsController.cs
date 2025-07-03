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
using Nutrition.Application.Features.Meal.Queries.Get;
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
        public async Task<HttpResult<GetMealResult>> Get(int id)
        {
            GetMealQuery query = new(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetMealResult>.Ok(result.Value!)
                : HttpResult<GetMealResult>.NotFound(result.Error!);
        }

        [HttpGet("diary/{id:int}")]
        public async Task<HttpResult<GetByDiaryResult>> GetByDiary(int id)
        {
            GetByDiaryQuery query = new(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetByDiaryResult>.Ok(result.Value!)
                : result.Error.Contains("NotFound")
                    ? HttpResult<GetByDiaryResult>.NotFound(result.Error!)
                    : HttpResult<GetByDiaryResult>.InternalError(result.Error!);
        }

        [HttpGet]
        public async Task<HttpResult<GetMealsResult>> GetAll([FromQuery] GetMealsQuery query)
        {
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetMealsResult>.Ok(result.Value!)
                : HttpResult<GetMealsResult>.InternalError(result.Error!);
        }

        [HttpPost("{mealId}/foods")]
        public async Task<HttpResult<AddMealFoodResult>> AddMealFood(int mealId, [FromBody] CreateMealFoodDTO dto, CancellationToken cancellationToken)
        {
            AddMealFoodCommand command = new(mealId, dto);
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<AddMealFoodResult>.Ok(result.Value!)
                : result.Error.Contains("NotFound")
                    ? HttpResult<AddMealFoodResult>.NotFound(result.Error!)
                    : HttpResult<AddMealFoodResult>.BadRequest(result.Error!);
        }

        [HttpDelete("{mealId}/foods/{foodId}")]
        public async Task<HttpResult<RemoveMealFoodResult>> RemoveMealFood(int mealId, int foodId, CancellationToken cancellationToken)
        {
            RemoveMealFoodCommand command = new(mealId, foodId);
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<RemoveMealFoodResult>.Ok(result.Value!)
                : result.Error.Contains("NotFound")
                    ? HttpResult<RemoveMealFoodResult>.NotFound(result.Error!)
                    : HttpResult<RemoveMealFoodResult>.BadRequest(result.Error!);
        }

        [HttpPost]
        public async Task<HttpResult<CreateMealResult>> Create([FromBody] CreateMealCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<CreateMealResult>.Created(result.Value!)
                : HttpResult<CreateMealResult>.BadRequest(result.Error!);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateMealResult>> Update(int id, [FromBody] UpdateMealCommand command)
        {
            UpdateMealCommand updateMealCommand = new(
                id,
                command.Name,
                command.Description);
            var result = await _sender.Send(updateMealCommand);

            return result.IsSuccess
                ? HttpResult<UpdateMealResult>.Ok(result.Value!)
                : result.Error.Contains("NotFound")
                    ? HttpResult<UpdateMealResult>.NotFound(result.Error!)
                    : HttpResult<UpdateMealResult>.BadRequest(result.Error!);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteMealResult>> Delete(int id)
        {
            DeleteMealCommand command = new(id);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<DeleteMealResult>.Deleted()
                : HttpResult<DeleteMealResult>.NotFound(result.Error!);
        }
    }
}
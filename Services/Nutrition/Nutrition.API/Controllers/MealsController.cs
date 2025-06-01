using BuildingBlocks.Results;
using Core.API.Controllers;
using BuildingBlocks.CQRS.Request;
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
using BuildingBlocks.CQRS.Sender;

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
            GetMealResult result = await _sender.Send(query);
            return HttpResult<GetMealResult>.Ok(result);
        }

        [HttpGet("diary/{id:int}")]
        public async Task<HttpResult<GetByDiaryResult>> GetByDiary(int id)
        {
            GetByDiaryQuery query = new(id);
            GetByDiaryResult result = await _sender.Send(query);
            return HttpResult<GetByDiaryResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetMealsResult>> GetAll([FromQuery] GetMealsQuery query)
        {
            GetMealsResult result = await _sender.Send(query);
            return HttpResult<GetMealsResult>.Ok(result);
        }

        [HttpPost("{mealId}/foods")]
        public async Task<HttpResult<AddMealFoodResult>> AddMealFood(int mealId, [FromBody] CreateMealFoodDTO dto, CancellationToken cancellationToken)
        {
            AddMealFoodCommand command = new(mealId, dto);
            AddMealFoodResult result = await _sender.Send(command, cancellationToken);
            return HttpResult<AddMealFoodResult>.Ok(result);
        }

        [HttpPost("{mealId}/foods/{foodId}")]
        public async Task<HttpResult<RemoveMealFoodResult>> RemoveMealFood(int mealId, int foodId, CancellationToken cancellationToken)
        {
            RemoveMealFoodCommand command = new(mealId, foodId);
            RemoveMealFoodResult result = await _sender.Send(command, cancellationToken);
            return HttpResult<RemoveMealFoodResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateMealResult>> Create([FromBody] CreateMealCommand command)
        {
            CreateMealResult result = await _sender.Send(command);
            return HttpResult<CreateMealResult>.Created(result);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateMealResult>> Update(int id, [FromBody] UpdateMealCommand command)
        {
            UpdateMealCommand updateMealCommand = new(
                id,
                command.Name,
                command.Description);
            UpdateMealResult result = await _sender.Send(updateMealCommand);
            return HttpResult<UpdateMealResult>.Updated();
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteMealResult>> Delete(int id)
        {
            DeleteMealCommand command = new(id);
            DeleteMealResult result = await _sender.Send(command);
            return HttpResult<DeleteMealResult>.Deleted();
        }
    }
}

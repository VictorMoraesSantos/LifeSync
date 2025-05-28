using BuildingBlocks.Results;
using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.Features.Meal.MealFood.Commands.Create;
using Nutrition.Application.Features.Meal.MealFood.Commands.Delete;
using Nutrition.Application.Features.Meal.MealFood.Commands.Update;
using Nutrition.Application.Features.Meal.MealFood.Queries.Get;
using Nutrition.Application.Features.Meal.MealFood.Queries.GetAll;
using Nutrition.Application.Features.Meal.MealFood.Queries.GetByMeal;

namespace Nutrition.API.Controllers
{
    public class MealsFoodController : ApiController
    {
        private readonly IMediator _mediator;

        public MealsFoodController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetMealFoodResult>> Get(int id)
        {
            GetMealFoodQuery query = new(id);
            GetMealFoodResult result = await _mediator.Send(query);
            return HttpResult<GetMealFoodResult>.Ok(result);
        }

        [HttpGet("meal/{id:int}")]
        public async Task<HttpResult<GetByMealResult>> GetByMeal(int id)
        {
            GetByMealQuery query = new(id);
            GetByMealResult result = await _mediator.Send(query);
            return HttpResult<GetByMealResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetMealFoodsResult>> GetAll([FromQuery] GetMealFoodsQuery query)
        {
            GetMealFoodsResult result = await _mediator.Send(query);
            return HttpResult<GetMealFoodsResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateMealFoodResult>> Create([FromBody] CreateMealFoodCommand command)
        {
            CreateMealFoodResult result = await _mediator.Send(command);
            return HttpResult<CreateMealFoodResult>.Created(result);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateMealFoodResult>> Update(int id, [FromBody] UpdateMealFoodCommand command)
        {
            UpdateMealFoodCommand updateMealFoodCommand = new(
                id,
                command.Name,
                command.QuantityInGrams,
                command.CaloriesPerUnit);
            UpdateMealFoodResult result = await _mediator.Send(updateMealFoodCommand);
            return HttpResult<UpdateMealFoodResult>.Updated();
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteMealFoodResult>> Delete(int id)
        {
            DeleteMealFoodCommand command = new(id);
            DeleteMealFoodResult result = await _mediator.Send(command);
            return HttpResult<DeleteMealFoodResult>.Deleted();
        }
    }
}

using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.Features.MealFood.Commands.Create;
using Nutrition.Application.Features.MealFood.Commands.Delete;
using Nutrition.Application.Features.MealFood.Commands.Update;
using Nutrition.Application.Features.MealFood.Queries.Get;
using Nutrition.Application.Features.MealFood.Queries.GetAll;
using Nutrition.Application.Features.MealFood.Queries.GetByMeal;

namespace Nutrition.API.Controllers
{
    public class MealsFoodController : ApiController
    {
        private readonly ISender _sender;

        public MealsFoodController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetMealFoodResult>> Get(int id)
        {
            GetMealFoodQuery query = new(id);
            GetMealFoodResult result = await _sender.Send(query);
            return HttpResult<GetMealFoodResult>.Ok(result);
        }

        [HttpGet("meal/{id:int}")]
        public async Task<HttpResult<GetByMealResult>> GetByMeal(int id)
        {
            GetByMealQuery query = new(id);
            GetByMealResult result = await _sender.Send(query);
            return HttpResult<GetByMealResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetMealFoodsResult>> GetAll([FromQuery] GetMealFoodsQuery query)
        {
            GetMealFoodsResult result = await _sender.Send(query);
            return HttpResult<GetMealFoodsResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateMealFoodResult>> Create([FromBody] CreateMealFoodCommand command)
        {
            CreateMealFoodResult result = await _sender.Send(command);
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
            UpdateMealFoodResult result = await _sender.Send(updateMealFoodCommand);
            return HttpResult<UpdateMealFoodResult>.Updated();
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteMealFoodResult>> Delete(int id)
        {
            DeleteMealFoodCommand command = new(id);
            DeleteMealFoodResult result = await _sender.Send(command);
            return HttpResult<DeleteMealFoodResult>.Deleted();
        }
    }
}

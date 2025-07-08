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
    [Route("api/v1/meals-food")]
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
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetMealFoodResult>.Ok(result.Value!)
                : HttpResult<GetMealFoodResult>.NotFound(result.Error!.Description);
        }

        [HttpGet("meal/{id:int}")]
        public async Task<HttpResult<GetByMealResult>> GetByMeal(int id)
        {
            GetByMealQuery query = new(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetByMealResult>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<GetByMealResult>.NotFound(result.Error!.Description!)
                    : HttpResult<GetByMealResult>.InternalError(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<GetMealFoodsResult>> GetAll([FromQuery] GetMealFoodsQuery query)
        {
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetMealFoodsResult>.Ok(result.Value!)
                : HttpResult<GetMealFoodsResult>.InternalError(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<CreateMealFoodResult>> Create([FromBody] CreateMealFoodCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<CreateMealFoodResult>.Created(result.Value!)
                : HttpResult<CreateMealFoodResult>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateMealFoodResult>> Update(int id, [FromBody] UpdateMealFoodCommand command)
        {
            UpdateMealFoodCommand updateMealFoodCommand = new(
                id,
                command.Name,
                command.QuantityInGrams,
                command.CaloriesPerUnit);
            var result = await _sender.Send(updateMealFoodCommand);

            return result.IsSuccess
                ? HttpResult<UpdateMealFoodResult>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<UpdateMealFoodResult>.NotFound(result.Error!.Description)
                    : HttpResult<UpdateMealFoodResult>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteMealFoodResult>> Delete(int id)
        {
            DeleteMealFoodCommand command = new(id);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<DeleteMealFoodResult>.Deleted()
                : HttpResult<DeleteMealFoodResult>.NotFound(result.Error!.Description);
        }
    }
}
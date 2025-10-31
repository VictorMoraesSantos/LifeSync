using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.DTOs.MealFood;
using Nutrition.Application.Features.MealFood.Commands.Create;
using Nutrition.Application.Features.MealFood.Commands.Delete;
using Nutrition.Application.Features.MealFood.Commands.Update;
using Nutrition.Application.Features.MealFood.Queries.GetAll;
using Nutrition.Application.Features.MealFood.Queries.GetByFilter;
using Nutrition.Application.Features.MealFood.Queries.GetById;
using Nutrition.Application.Features.MealFood.Queries.GetByMeal;

namespace Nutrition.API.Controllers
{
    [Route("api/meals-food")]
    public class MealsFoodController : ApiController
    {
        private readonly ISender _sender;

        public MealsFoodController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> Get(int id)
        {
            GetMealFoodQuery query = new(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("meal/{id:int}")]
        public async Task<HttpResult<object>> GetByMeal(int id)
        {
            GetByMealQuery query = new(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description!)
                    : HttpResult<object>.InternalError(result.Error!.Description);
        }

        [HttpGet("search")]
        public async Task<HttpResult<object>> Search([FromQuery] MealFoodQueryFilterDTO filter, CancellationToken cancellationToken)
        {
            var query = new GetByFilterQuery(filter);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value?.Items!, result.Value?.Pagination!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll([FromQuery] GetMealFoodsQuery query)
        {
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.InternalError(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create([FromBody] CreateMealFoodCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update(int id, [FromBody] UpdateMealFoodCommand command)
        {
            UpdateMealFoodCommand updateMealFoodCommand = new(
                id,
                command.Name,
                command.QuantityInGrams,
                command.CaloriesPerUnit);
            var result = await _sender.Send(updateMealFoodCommand);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description)
                    : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id)
        {
            DeleteMealFoodCommand command = new(id);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.NotFound(result.Error!.Description);
        }
    }
}
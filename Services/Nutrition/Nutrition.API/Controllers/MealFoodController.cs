using BuildingBlocks.Results;
using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.UseCases.MealFood.Commands.Create;
using Nutrition.Application.UseCases.MealFood.Commands.Delete;
using Nutrition.Application.UseCases.MealFood.Commands.Update;
using Nutrition.Application.UseCases.MealFood.Queries.Get;
using Nutrition.Application.UseCases.MealFood.Queries.GetAll;
using Nutrition.Application.UseCases.MealFood.Queries.GetByMeal;

namespace Nutrition.API.Controllers
{
    public class MealFoodController : ApiController
    {
        private readonly IMediator _mediator;

        public MealFoodController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<HttpResult<GetMealFoodResponse>> Get(int id)
        {
            GetMealFoodQuery query = new(id);
            var result = await _mediator.Send(query);
            return HttpResult<GetMealFoodResponse>.Ok(result);
        }

        [HttpGet("meal/{id}")]
        public async Task<HttpResult<GetByMealResponse>> GetByMeal(int id)
        {
            GetByMealQuery query = new(id);
            var result = await _mediator.Send(query);
            return HttpResult<GetByMealResponse>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetMealFoodsResponse>> GetAll([FromQuery] GetMealFoodsQuery query)
        {
            var result = await _mediator.Send(query);
            return HttpResult<GetMealFoodsResponse>.Ok(result);
        }

        [HttpPost("create")]
        public async Task<HttpResult<CreateMealFoodResponse>> Create([FromBody] CreateMealFoodCommand command)
        {
            var result = await _mediator.Send(command);
            return HttpResult<CreateMealFoodResponse>.Created(result);
        }

        [HttpPut("update")]
        public async Task<HttpResult<UpdateMealFoodResponse>> Update([FromBody] UpdateMealFoodCommand command)
        {
            var result = await _mediator.Send(command);
            return HttpResult<UpdateMealFoodResponse>.Updated();
        }

        [HttpDelete("delete/{id}")]
        public async Task<HttpResult<DeleteMealFoodResponse>> Delete(int id)
        {
            DeleteMealFoodCommand command = new(id);
            var result = await _mediator.Send(command);
            return HttpResult<DeleteMealFoodResponse>.Deleted();
        }
    }
}

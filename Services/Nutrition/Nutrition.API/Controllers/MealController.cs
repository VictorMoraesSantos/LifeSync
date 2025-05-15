using BuildingBlocks.Results;
using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.DTOs.Meals;
using Nutrition.Application.UseCases.Meal.Commands.Create;
using Nutrition.Application.UseCases.Meal.Commands.Delete;
using Nutrition.Application.UseCases.Meal.Commands.Update;
using Nutrition.Application.UseCases.Meal.Queries.Get;
using Nutrition.Application.UseCases.Meal.Queries.GetAll;
using Nutrition.Application.UseCases.Meal.Queries.GetByDiary;

namespace Nutrition.API.Controllers
{
    public class MealController : ApiController
    {
        private readonly IMediator _mediator;

        public MealController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<HttpResult<GetMealQueryResponse>> Get(int id)
        {
            GetMealQuery query = new(id);
            var result = await _mediator.Send(query);
            return HttpResult<GetMealQueryResponse>.Ok(result);
        }

        [HttpGet("diary/{id}")]
        public async Task<HttpResult<GetByDiaryResponse>> GetByDiary(int id)
        {
            GetByDiaryQuery query = new(id);
            var result = await _mediator.Send(query);
            return HttpResult<GetByDiaryResponse>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetMealsQueryResponse>> GetAll([FromQuery] GetMealsQuery query)
        {
            var result = await _mediator.Send(query);
            return HttpResult<GetMealsQueryResponse>.Ok(result);
        }

        [HttpPost("create")]
        public async Task<HttpResult<CreateMealResponse>> Create([FromBody] CreateMealCommand command)
        {
            var result = await _mediator.Send(command);
            return HttpResult<CreateMealResponse>.Created(result);
        }

        [HttpPut("update")]
        public async Task<HttpResult<UpdateMealResponse>> Update([FromBody] UpdateMealCommand command)
        {
            var result = await _mediator.Send(command);
            return HttpResult<UpdateMealResponse>.Updated();
        }

        [HttpDelete("delete/{id}")]
        public async Task<HttpResult<DeleteMealResponse>> Delete(int id)
        {
            DeleteMealCommand command = new(id);
            var result = await _mediator.Send(command);
            return HttpResult<DeleteMealResponse>.Deleted();
        }
    }
}

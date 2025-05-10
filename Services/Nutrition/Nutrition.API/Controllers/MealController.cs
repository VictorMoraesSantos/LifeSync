using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.DTOs.Meals;
using Nutrition.Application.UseCases.Meals.Commands.Create;
using Nutrition.Application.UseCases.Meals.Commands.Delete;
using Nutrition.Application.UseCases.Meals.Commands.Update;
using Nutrition.Application.UseCases.Meals.Queries.Get;
using Nutrition.Application.UseCases.Meals.Queries.GetAll;
using Nutrition.Application.UseCases.Meals.Queries.GetByDiary;

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
        public async Task<ActionResult<MealDTO>> Get(int id)
        {
            GetMealQuery query = new(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("diary/{id}")]
        public async Task<ActionResult<IEnumerable<MealDTO>>> GetByDiary(int id)
        {
            GetByDiaryQuery query = new(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MealDTO>>> GetAll([FromQuery] GetMealsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<ActionResult<CreateMealResponse>> Create([FromBody] CreateMealCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update")]
        public async Task<ActionResult<UpdateMealResponse>> Update([FromBody] UpdateMealCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<DeleteMealResponse>> Delete(int id)
        {
            DeleteMealCommand command = new(id);
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.UseCases.MealFoods.Commands.Create;
using Nutrition.Application.UseCases.MealFoods.Commands.Update;

namespace Nutrition.API.Controllers
{
    public class MealFoodController : ApiController
    {
        private readonly IMediator _mediator;
        
        public MealFoodController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<ActionResult<CreateMealFoodResponse>> Create([FromBody] CreateMealFoodCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update")]
        public async Task<ActionResult<UpdateMealFoodResponse>> Update([FromBody] UpdateMealFoodCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        //[HttpDelete("delete/{id}")]
        //public async Task<ActionResult<DeleteMealFoodResponse>> Delete(int id)
        //{
        //    var command = new DeleteMealFoodCommand(id);
        //    var result = await _mediator.Send(command);
        //    return result.IsSuccess ? Ok(result) : BadRequest(result);
        //}
    }
}

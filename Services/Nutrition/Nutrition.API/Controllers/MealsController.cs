using BuildingBlocks.Results;
using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.UseCases.Meal.Commands.Create;
using Nutrition.Application.UseCases.Meal.Commands.Delete;
using Nutrition.Application.UseCases.Meal.Commands.Update;
using Nutrition.Application.UseCases.Meal.Queries.Get;
using Nutrition.Application.UseCases.Meal.Queries.GetAll;
using Nutrition.Application.UseCases.Meal.Queries.GetByDiary;

namespace Nutrition.API.Controllers
{
    public class MealsController : ApiController
    {
        private readonly IMediator _mediator;

        public MealsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetMealResult>> Get(int id)
        {
            GetMealQuery query = new(id);
            GetMealResult result = await _mediator.Send(query);
            return HttpResult<GetMealResult>.Ok(result);
        }

        [HttpGet("diary/{id:int}")]
        public async Task<HttpResult<GetByDiaryResult>> GetByDiary(int id)
        {
            GetByDiaryQuery query = new(id);
            GetByDiaryResult result = await _mediator.Send(query);
            return HttpResult<GetByDiaryResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetMealsResult>> GetAll([FromQuery] GetMealsQuery query)
        {
            GetMealsResult result = await _mediator.Send(query);
            return HttpResult<GetMealsResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateMealResult>> Create([FromBody] CreateMealCommand command)
        {
            CreateMealResult result = await _mediator.Send(command);
            return HttpResult<CreateMealResult>.Created(result);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateMealResult>> Update(int id, [FromBody] UpdateMealCommand command)
        {
            UpdateMealCommand updateMealCommand = new(
                id,
                command.Name,
                command.Description);
            UpdateMealResult result = await _mediator.Send(updateMealCommand);
            return HttpResult<UpdateMealResult>.Updated();
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteMealResult>> Delete(int id)
        {
            DeleteMealCommand command = new(id);
            DeleteMealResult result = await _mediator.Send(command);
            return HttpResult<DeleteMealResult>.Deleted();
        }
    }
}

using BuildingBlocks.Results;
using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.UseCases.Liquid.Commands.Create;
using Nutrition.Application.UseCases.Liquid.Commands.Delete;
using Nutrition.Application.UseCases.Liquid.Commands.Update;
using Nutrition.Application.UseCases.Liquid.Queries.Get;
using Nutrition.Application.UseCases.Liquid.Queries.GetAll;
using Nutrition.Application.UseCases.Liquid.Queries.GetByDiary;
using Nutrition.Application.UseCases.Meal.Queries.GetByDiary;

namespace Nutrition.API.Controllers
{
    public class LiquidsController : ApiController
    {
        private readonly IMediator _mediator;

        public LiquidsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetLiquidResult>> Get(int id)
        {
            GetLiquidQuery query = new(id);
            var result = await _mediator.Send(query);
            return HttpResult<GetLiquidResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllLiquidsResult>> GetAll([FromQuery] GetAllLiquidsQuery query)
        {
            var result = await _mediator.Send(query);
            return HttpResult<GetAllLiquidsResult>.Ok(result);
        }

        [HttpGet("diary/{diaryId:int}")]
        public async Task<HttpResult<GetLiquidsByDiaryResult>> GetByDiary(int diaryId)
        {
            GetLiquidsByDiaryQuery query = new(diaryId);
            var result = await _mediator.Send(query);
            return HttpResult<GetLiquidsByDiaryResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateLiquidResult>> Create([FromBody] CreateLiquidCommand command)
        {
            var result = await _mediator.Send(command);
            return HttpResult<CreateLiquidResult>.Created(result);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateLiquidResult>> Update([FromBody] UpdateLiquidCommand command, int id)
        {
            UpdateLiquidCommand updateLiquidCommand = new(
                id,
                command.Name,
                command.QuantityMl,
                command.CaloriesPerMl
            );
            var result = await _mediator.Send(updateLiquidCommand);
            return HttpResult<UpdateLiquidResult>.Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteLiquidResult>> Delete(int id)
        {
            DeleteLiquidCommand command = new(id);
            await _mediator.Send(command);
            return HttpResult<DeleteLiquidResult>.Deleted();
        }
    }
}

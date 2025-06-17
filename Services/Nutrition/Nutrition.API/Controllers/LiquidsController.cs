using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.Features.Liquid.Commands.Create;
using Nutrition.Application.Features.Liquid.Commands.Delete;
using Nutrition.Application.Features.Liquid.Commands.Update;
using Nutrition.Application.Features.Liquid.Queries.Get;
using Nutrition.Application.Features.Liquid.Queries.GetAll;
using Nutrition.Application.Features.Liquid.Queries.GetByDiary;

namespace Nutrition.API.Controllers
{
    public class LiquidsController : ApiController
    {
        private readonly ISender _sender;

        public LiquidsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<GetLiquidResult>> Get(int id)
        {
            GetLiquidQuery query = new(id);
            GetLiquidResult result = await _sender.Send(query);
            return HttpResult<GetLiquidResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllLiquidsResult>> GetAll([FromQuery] GetAllLiquidsQuery query)
        {
            GetAllLiquidsResult result = await _sender.Send(query);
            return HttpResult<GetAllLiquidsResult>.Ok(result);
        }

        [HttpGet("diary/{diaryId:int}")]
        public async Task<HttpResult<GetLiquidsByDiaryResult>> GetByDiary(int diaryId)
        {
            GetLiquidsByDiaryQuery query = new(diaryId);
            GetLiquidsByDiaryResult result = await _sender.Send(query);
            return HttpResult<GetLiquidsByDiaryResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateLiquidResult>> Create([FromBody] CreateLiquidCommand command)
        {
            CreateLiquidResult result = await _sender.Send(command);
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
            UpdateLiquidResult result = await _sender.Send(updateLiquidCommand);
            return HttpResult<UpdateLiquidResult>.Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteLiquidResult>> Delete(int id)
        {
            DeleteLiquidCommand command = new(id);
            await _sender.Send(command);
            return HttpResult<DeleteLiquidResult>.Deleted();
        }
    }
}

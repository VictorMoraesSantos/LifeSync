using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.Features.Liquid.Commands.Create;
using Nutrition.Application.Features.Liquid.Commands.Delete;
using Nutrition.Application.Features.Liquid.Commands.Update;
using Nutrition.Application.Features.Liquid.Queries.GetAll;
using Nutrition.Application.Features.Liquid.Queries.GetByDiary;
using Nutrition.Application.Features.Liquid.Queries.GetById;

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
        public async Task<HttpResult<object>> Get(int id)
        {
            GetLiquidQuery query = new(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll([FromQuery] GetAllLiquidsQuery query)
        {
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.InternalError(result.Error!.Description);
        }

        [HttpGet("diary/{diaryId:int}")]
        public async Task<HttpResult<object>> GetByDiary(int diaryId)
        {
            GetLiquidsByDiaryQuery query = new(diaryId);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description)
                    : HttpResult<object>.InternalError(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create([FromBody] CreateLiquidCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update([FromBody] UpdateLiquidCommand command, int id)
        {
            UpdateLiquidCommand updateLiquidCommand = new(
                id,
                command.Name,
                command.QuantityMl,
                command.CaloriesPerMl
            );
            var result = await _sender.Send(updateLiquidCommand);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description)
                    : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id)
        {
            DeleteLiquidCommand command = new(id);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.NotFound(result.Error!.Description);
        }
    }
}
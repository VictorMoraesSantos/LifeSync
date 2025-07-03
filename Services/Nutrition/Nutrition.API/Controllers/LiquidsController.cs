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
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetLiquidResult>.Ok(result.Value!)
                : HttpResult<GetLiquidResult>.NotFound(result.Error!);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllLiquidsResult>> GetAll([FromQuery] GetAllLiquidsQuery query)
        {
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetAllLiquidsResult>.Ok(result.Value!)
                : HttpResult<GetAllLiquidsResult>.InternalError(result.Error!);
        }

        [HttpGet("diary/{diaryId:int}")]
        public async Task<HttpResult<GetLiquidsByDiaryResult>> GetByDiary(int diaryId)
        {
            GetLiquidsByDiaryQuery query = new(diaryId);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<GetLiquidsByDiaryResult>.Ok(result.Value!)
                : result.Error.Contains("NotFound")
                    ? HttpResult<GetLiquidsByDiaryResult>.NotFound(result.Error!)
                    : HttpResult<GetLiquidsByDiaryResult>.InternalError(result.Error!);
        }

        [HttpPost]
        public async Task<HttpResult<CreateLiquidResult>> Create([FromBody] CreateLiquidCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<CreateLiquidResult>.Created(result.Value!)
                : HttpResult<CreateLiquidResult>.BadRequest(result.Error!);
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
            var result = await _sender.Send(updateLiquidCommand);

            return result.IsSuccess
                ? HttpResult<UpdateLiquidResult>.Ok(result.Value!)
                : result.Error.Contains("NotFound")
                    ? HttpResult<UpdateLiquidResult>.NotFound(result.Error!)
                    : HttpResult<UpdateLiquidResult>.BadRequest(result.Error!);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteLiquidResult>> Delete(int id)
        {
            DeleteLiquidCommand command = new(id);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<DeleteLiquidResult>.Deleted()
                : HttpResult<DeleteLiquidResult>.NotFound(result.Error!);
        }
    }
}
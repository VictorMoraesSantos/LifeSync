using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.DTOs.LiquidType;
using Nutrition.Application.Features.LiquidType.Commands.Create;
using Nutrition.Application.Features.LiquidType.Commands.Delete;
using Nutrition.Application.Features.LiquidType.Commands.Update;
using Nutrition.Application.Features.LiquidType.Queries.GetAll;
using Nutrition.Application.Features.LiquidType.Queries.GetByFilter;
using Nutrition.Application.Features.LiquidType.Queries.GetById;

namespace Nutrition.API.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/liquid-types")]
    public class LiquidTypesController : ApiController
    {
        private readonly ISender _sender;

        public LiquidTypesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> Get(int id)
        {
            GetLiquidTypeQuery query = new(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.LiquidType)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll([FromQuery] GetAllLiquidTypesQuery query)
        {
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.LiquidTypes)
                : HttpResult<object>.InternalError(result.Error!.Description);
        }

        [HttpGet("search")]
        public async Task<HttpResult<object>> Search([FromQuery] LiquidTypeQueryFilterDTO filter, CancellationToken cancellationToken)
        {
            var query = new GetLiquidTypeByFilterQuery(filter);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value?.Items!, result.Value?.Pagination!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> Create([FromBody] CreateLiquidTypeCommand command)
        {
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!.Id)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> Update([FromBody] UpdateLiquidTypeCommand command, int id)
        {
            var updateLiquidTypeCommand = new UpdateLiquidTypeCommand(
                id,
                command.Name);
            var result = await _sender.Send(updateLiquidTypeCommand);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.IsSuccess)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description)
                    : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> Delete(int id)
        {
            DeleteLiquidTypeCommand command = new(id);
            var result = await _sender.Send(command);

            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.NotFound(result.Error!.Description);
        }
    }
}

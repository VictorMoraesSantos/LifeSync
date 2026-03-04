using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Nutrition.Application.DTOs.Food;
using Nutrition.Application.Features.Food.Queries.GetAll;
using Nutrition.Application.Features.Food.Queries.GetByFilter;
using Nutrition.Application.Features.Food.Queries.GetById;

namespace Nutrition.API.Controllers
{
    public class FoodsController : ApiController
    {
        private readonly ISender _sender;

        public FoodsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> GetById(int id)
        {
            GetFoodByIdQuery query = new GetFoodByIdQuery(id);
            var result = await _sender.Send(query);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Food)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("search")]
        public async Task<HttpResult<object>> Search([FromQuery] FoodQueryFilterDTO filter, CancellationToken cancellationToken)
        {
            var query = new GetFoodByFilterQuery(filter);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value?.Items!, result.Value?.Pagination!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAll([FromQuery] GetAllFoodsQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value?.Foods!)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }
    }
}

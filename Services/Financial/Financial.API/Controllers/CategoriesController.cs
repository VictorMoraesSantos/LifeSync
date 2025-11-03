using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Financial.Application.Features.Categories.Commands.Create;
using Financial.Application.Features.Categories.Commands.Delete;
using Financial.Application.Features.Categories.Commands.Update;
using Financial.Application.Features.Categories.Queries.GetAll;
using Financial.Application.Features.Categories.Queries.GetById;
using Financial.Application.Features.Categories.Queries.GetByUserId;
using Microsoft.AspNetCore.Mvc;

namespace Financial.API.Controllers
{
    public class CategoriesController : ApiController
    {
        private readonly ISender _sender;

        public CategoriesController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Category)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<object>> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            var query = new GetCategoriesByUserIdQuery(userId);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Categories)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAllAsync(CancellationToken cancellationToken)
        {
            var query = new GetAllCategoriesQuery();
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Categories)
                : HttpResult<object>.InternalError(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> CreateAsync([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!.Id)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> UpdateAsync(int id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            var update = new UpdateCategoryCommand(id, command.Name, command.Description);
            var result = await _sender.Send(update, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.IsSuccess)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description)
                    : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.NotFound(result.Error!.Description);
        }
    }
}
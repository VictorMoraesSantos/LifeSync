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
        public async Task<HttpResult<GetCategoryByIdResult>> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<GetCategoryByIdResult>.Ok(result.Value!)
                : HttpResult<GetCategoryByIdResult>.NotFound(result.Error!.Description);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<GetCategoriesByUserIdResult>> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            var query = new GetCategoriesByUserIdQuery(userId);
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<GetCategoriesByUserIdResult>.Ok(result.Value!)
                : HttpResult<GetCategoriesByUserIdResult>.NotFound(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllCategoriesResult>> GetAllAsync(CancellationToken cancellationToken)
        {
            var query = new GetAllCategoriesQuery();
            var result = await _sender.Send(query, cancellationToken);

            return result.IsSuccess
                ? HttpResult<GetAllCategoriesResult>.Ok(result.Value!)
                : HttpResult<GetAllCategoriesResult>.InternalError(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<CreateCategoryResult>> CreateAsync([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<CreateCategoryResult>.Created(result.Value!)
                : HttpResult<CreateCategoryResult>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateCategoryResult>> UpdateAsync(int id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            UpdateCategoryCommand update = new(id, command.Name, command.Description);
            var result = await _sender.Send(update, cancellationToken);

            return result.IsSuccess
                ? HttpResult<UpdateCategoryResult>.Ok(result.Value!)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<UpdateCategoryResult>.NotFound(result.Error!.Description)
                    : HttpResult<UpdateCategoryResult>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteCategoryResult>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand(id);
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<DeleteCategoryResult>.Deleted()
                : HttpResult<DeleteCategoryResult>.NotFound(result.Error!.Description);
        }
    }
}
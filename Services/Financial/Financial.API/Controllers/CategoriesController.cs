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
        public async Task<HttpResult<GetCategoryByIdResult>> GetAllAsync(int id, CancellationToken cancellationToken)
        {
            var query = new GetCategoryByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return HttpResult<GetCategoryByIdResult>.Ok(result);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<HttpResult<GetCategoriesByUserIdResult>> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            var query = new GetCategoriesByUserIdQuery(userId);
            var result = await _sender.Send(query, cancellationToken);
            return HttpResult<GetCategoriesByUserIdResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllCategoriesResult>> GetAllAsync(CancellationToken cancellationToken)
        {
            var query = new GetAllCategoriesQuery();
            var result = await _sender.Send(query, cancellationToken);
            return HttpResult<GetAllCategoriesResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateCategoryResult>> CreateAsync([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return HttpResult<CreateCategoryResult>.Created(result);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateCategoryResult>> UpdateAsync(int id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            UpdateCategoryCommand update = new(id, command.Name, command.Description);
            var result = await _sender.Send(update, cancellationToken);
            return HttpResult<UpdateCategoryResult>.Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteCategoryResult>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteCategoryCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return HttpResult<DeleteCategoryResult>.Ok(result);
        }
    }
}

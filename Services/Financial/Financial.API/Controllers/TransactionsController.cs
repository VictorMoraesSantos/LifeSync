using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Financial.Application.DTOs.Transaction;
using Financial.Application.Features.Transactions.Commands.Create;
using Financial.Application.Features.Transactions.Commands.Delete;
using Financial.Application.Features.Transactions.Commands.Update;
using Financial.Application.Features.Transactions.Queries.GetAll;
using Financial.Application.Features.Transactions.Queries.GetByFilter;
using Financial.Application.Features.Transactions.Queries.GetById;
using Financial.Application.Features.Transactions.Queries.GetByUserId;
using Microsoft.AspNetCore.Mvc;

namespace Financial.API.Controllers
{
    public class TransactionsController : ApiController
    {
        private readonly ISender _sender;

        public TransactionsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:int}")]
        public async Task<HttpResult<object>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var query = new GetTransactionByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Transaction)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("user/{id:int}")]
        public async Task<HttpResult<object>> GetByUserIdAsync(int id, CancellationToken cancellationToken)
        {
            var query = new GetTransactionsByUserIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Transactions)
                : HttpResult<object>.NotFound(result.Error!.Description);
        }

        [HttpGet("search")]
        public async Task<HttpResult<object>> Search([FromQuery] TransactionFilterDTO filter, CancellationToken cancellationToken)
        {
            var query = new GetTransactionsByFilterQuery(filter);
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value.Items, result.Value.Pagination)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpGet]
        public async Task<HttpResult<object>> GetAllAsync(CancellationToken cancellationToken)
        {
            var query = new GetAllTransactionsQuery();
            var result = await _sender.Send(query, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.Transactions)
                : HttpResult<object>.InternalError(result.Error!.Description);
        }

        [HttpPost]
        public async Task<HttpResult<object>> CreateAsync([FromBody] CreateTransactionCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Created(result.Value!.TransactionId)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<object>> UpdateAsync(int id, [FromBody] UpdateTransactionCommand command, CancellationToken cancellationToken)
        {
            var updatedCommand = new UpdateTransactionCommand(
                id,
                command.CategoryId,
                command.PaymentMethod,
                command.TransactionType,
                command.Amount,
                command.Description,
                command.TransactionDate);
            var result = await _sender.Send(updatedCommand, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!.IsSuccess)
                : result.Error!.Description.Contains("NotFound")
                    ? HttpResult<object>.NotFound(result.Error!.Description)
                    : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<object>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteTransactionCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Deleted()
                : HttpResult<object>.NotFound(result.Error!.Description);
        }
    }
}
using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Financial.Application.Features.Transactions.Commands.Create;
using Financial.Application.Features.Transactions.Commands.Delete;
using Financial.Application.Features.Transactions.Commands.Update;
using Financial.Application.Features.Transactions.Queries.GetAll;
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
        public async Task<HttpResult<GetTransactionByIdResult>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var query = new GetTransactionByIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return HttpResult<GetTransactionByIdResult>.Ok(result);
        }

        [HttpGet("user/{id:int}")]
        public async Task<HttpResult<GetTransactionsByUserIdResult>> GetByUserIdAsync(int id, CancellationToken cancellationToken)
        {
            var query = new GetTransactionsByUserIdQuery(id);
            var result = await _sender.Send(query, cancellationToken);
            return HttpResult<GetTransactionsByUserIdResult>.Ok(result);
        }

        [HttpGet]
        public async Task<HttpResult<GetAllTransactionsResult>> GetAllAsync(CancellationToken cancellationToken)
        {
            var query = new GetAllTransactionsQuery();
            var result = await _sender.Send(query, cancellationToken);
            return HttpResult<GetAllTransactionsResult>.Ok(result);
        }

        [HttpPost]
        public async Task<HttpResult<CreateTransactionResult>> CreateAsync([FromBody] CreateTransactionCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return HttpResult<CreateTransactionResult>.Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<HttpResult<UpdateTransactionResult>> UpdateAsync(int id, [FromBody] UpdateTransactionCommand command, CancellationToken cancellationToken)
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
            return HttpResult<UpdateTransactionResult>.Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<HttpResult<DeleteTransactionResult>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteTransactionCommand(id);
            var result = await _sender.Send(command, cancellationToken);
            return HttpResult<DeleteTransactionResult>.Ok(result);
        }
    }
}

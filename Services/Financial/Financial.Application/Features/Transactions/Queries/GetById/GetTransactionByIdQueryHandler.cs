using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Transactions.Queries.GetById
{
    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, GetTransactionByIdResult>
    {
        private readonly ITransactionService _transactionService;

        public GetTransactionByIdQueryHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<GetTransactionByIdResult> Handle(GetTransactionByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _transactionService.GetByIdAsync(query.Id, cancellationToken);
            return new GetTransactionByIdResult(result);
        }
    }
}

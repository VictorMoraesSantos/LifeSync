using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Transactions.Queries.GetAll
{
    public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, GetAllTransactionsResult>
    {
        private readonly ITransactionService _transactionService;

        public GetAllTransactionsQueryHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<GetAllTransactionsResult> Handle(GetAllTransactionsQuery query, CancellationToken cancellationToken)
        {
            var result = await _transactionService.GetAllAsync(cancellationToken);
            return new GetAllTransactionsResult(result);
        }
    }
}

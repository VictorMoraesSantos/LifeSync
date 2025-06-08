using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Transactions.Queries.GetByUserId
{
    public class GetTransactionsByUserIdQueryHandler : IRequestHandler<GetTransactionsByUserIdQuery, GetTransactionsByUserIdResult>
    {
        private readonly ITransactionService _transactionService;

        public GetTransactionsByUserIdQueryHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<GetTransactionsByUserIdResult> Handle(GetTransactionsByUserIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _transactionService.GetByUserIdAsync(query.UserId, cancellationToken);
            return new GetTransactionsByUserIdResult(result);
        }
    }
}

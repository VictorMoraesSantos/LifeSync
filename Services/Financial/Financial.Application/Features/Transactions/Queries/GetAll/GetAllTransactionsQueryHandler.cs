using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Transactions.Queries.GetAll
{
    public class GetAllTransactionsQueryHandler : IQueryHandler<GetAllTransactionsQuery, GetAllTransactionsResult>
    {
        private readonly ITransactionService _transactionService;

        public GetAllTransactionsQueryHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<Result<GetAllTransactionsResult>> Handle(GetAllTransactionsQuery query, CancellationToken cancellationToken)
        {
            var result = await _transactionService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetAllTransactionsResult>(result.Error!);

            return Result.Success(new GetAllTransactionsResult(result.Value!));
        }
    }
}

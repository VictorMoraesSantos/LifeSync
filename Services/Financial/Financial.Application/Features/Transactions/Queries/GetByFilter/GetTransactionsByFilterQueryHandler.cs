using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Transactions.Queries.GetByFilter
{
    public class GetTransactionsByFilterQueryHandler : IQueryHandler<GetTransactionsByFilterQuery, GetTransactionsByFilterResult>
    {
        private readonly ITransactionService _transactionService;

        public GetTransactionsByFilterQueryHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<Result<GetTransactionsByFilterResult>> Handle(GetTransactionsByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _transactionService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetTransactionsByFilterResult>(result.Error!);

            return Result.Success(new GetTransactionsByFilterResult(result.Value.Items, result.Value.Pagination));
        }
    }
}

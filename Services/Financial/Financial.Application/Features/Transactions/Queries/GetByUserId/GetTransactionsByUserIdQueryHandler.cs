using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Transactions.Queries.GetByUserId
{
    public class GetTransactionsByUserIdQueryHandler : IQueryHandler<GetTransactionsByUserIdQuery, GetTransactionsByUserIdResult>
    {
        private readonly ITransactionService _transactionService;

        public GetTransactionsByUserIdQueryHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<Result<GetTransactionsByUserIdResult>> Handle(GetTransactionsByUserIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _transactionService.GetByUserIdAsync(query.UserId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetTransactionsByUserIdResult>(result.Error!);

            return Result.Success(new GetTransactionsByUserIdResult(result.Value!));
        }
    }
}

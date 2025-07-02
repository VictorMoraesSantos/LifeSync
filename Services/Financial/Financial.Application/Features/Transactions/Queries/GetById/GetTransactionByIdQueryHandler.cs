using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Transactions.Queries.GetById
{
    public class GetTransactionByIdQueryHandler : IQueryHandler<GetTransactionByIdQuery, GetTransactionByIdResult>
    {
        private readonly ITransactionService _transactionService;

        public GetTransactionByIdQueryHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<Result<GetTransactionByIdResult>> Handle(GetTransactionByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _transactionService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetTransactionByIdResult>(result.Error!);

            return Result.Success(new GetTransactionByIdResult(result.Value!));
        }
    }
}

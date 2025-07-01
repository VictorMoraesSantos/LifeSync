using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;

namespace BuildingBlocks.CQRS.Handlers
{
    public abstract class QueryHandler<TQuery, TResponse>
        : IRequestHandler<TQuery, Result<TResponse>>
        where TQuery : IQuery<TResponse>
    {
        public abstract Task<Result<TResponse>> Handle(TQuery request, CancellationToken cancellationToken);
    }
}

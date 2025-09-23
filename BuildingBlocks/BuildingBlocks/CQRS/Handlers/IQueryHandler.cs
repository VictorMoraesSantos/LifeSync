using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;

namespace BuildingBlocks.CQRS.Handlers
{
    public interface IQueryHandler<TQuery, TResponse>
        : IRequestHandler<TQuery, Result<TResponse>>
         where TQuery : IQuery<TResponse>
    {
    }
}

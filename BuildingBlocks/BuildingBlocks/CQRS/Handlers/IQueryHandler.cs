using BuildingBlocks.CQRS.Requests.Queries;
using BuildingBlocks.CQRS.Requests.Request;
using BuildingBlocks.Results;

namespace BuildingBlocks.CQRS.Handlers
{
    public interface IQueryHandler<TQuery, TResponse>
        : IRequestHandler<TQuery, Result<TResponse>>
         where TQuery : IQuery<TResponse>
    {
    }
}

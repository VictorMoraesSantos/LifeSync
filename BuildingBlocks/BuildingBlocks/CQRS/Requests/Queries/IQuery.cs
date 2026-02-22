using BuildingBlocks.CQRS.Requests.Request;
using BuildingBlocks.Results;

namespace BuildingBlocks.CQRS.Requests.Queries
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
}

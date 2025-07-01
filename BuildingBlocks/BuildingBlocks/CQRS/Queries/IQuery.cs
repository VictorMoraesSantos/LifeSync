using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;

namespace BuildingBlocks.CQRS.Queries
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
}

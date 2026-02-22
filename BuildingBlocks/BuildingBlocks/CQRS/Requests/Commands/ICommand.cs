using BuildingBlocks.CQRS.Requests.Request;
using BuildingBlocks.Results;

namespace BuildingBlocks.CQRS.Requests.Commands
{
    public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
    public interface ICommand : IRequest<Result> { }
}

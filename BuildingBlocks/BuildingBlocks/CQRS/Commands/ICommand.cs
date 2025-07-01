using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;

namespace BuildingBlocks.CQRS.Commands
{
    public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }
    public interface ICommand : IRequest<Result> { }
}

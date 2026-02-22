using BuildingBlocks.CQRS.Requests.Commands;
using BuildingBlocks.CQRS.Requests.Request;
using BuildingBlocks.Results;

namespace BuildingBlocks.CQRS.Handlers
{
    public interface ICommandHandler<TCommand, TResponse>
        : IRequestHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>
    {
    }

    public interface ICommandHandler<TCommand>
        : IRequestHandler<TCommand, Result>
        where TCommand : ICommand
    {
    }
}

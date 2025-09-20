using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;
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

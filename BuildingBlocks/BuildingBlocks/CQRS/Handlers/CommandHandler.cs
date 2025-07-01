using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;

namespace BuildingBlocks.CQRS.Handlers
{
    public abstract class CommandHandler<TCommand, TResponse>
        : IRequestHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>
    {
        public abstract Task<Result<TResponse>> Handle(TCommand request, CancellationToken cancellationToken);
    }

    public abstract class CommandHandler<TCommand>
        : IRequestHandler<TCommand, Result>
        where TCommand : ICommand
    {
        public abstract Task<Result> Handle(TCommand request, CancellationToken cancellationToken);
    }
}

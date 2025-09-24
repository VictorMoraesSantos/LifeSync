using BuildingBlocks.CQRS.Request;

namespace BuildingBlocks.CQRS.Pipeline
{
    public interface IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(
            TRequest request,
            Func<Task<TResponse>> next,
            CancellationToken cancellationToken);
    }
}
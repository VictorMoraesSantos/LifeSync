using BuildingBlocks.CQRS.Request;

namespace BuildingBlocks.CQRS.Sender
{
    public interface ISender
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }
}

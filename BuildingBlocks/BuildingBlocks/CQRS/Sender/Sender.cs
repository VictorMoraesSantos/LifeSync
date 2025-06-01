using BuildingBlocks.CQRS.Request;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.CQRS.Sender
{
    public class Sender : ISender
    {
        private readonly IServiceProvider _serviceProvider;

        public Sender(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            dynamic handler = _serviceProvider.GetRequiredService(handlerType);
            return handler.Handle((dynamic)request, cancellationToken);
        }
    }
}

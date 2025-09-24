using BuildingBlocks.CQRS.Pipeline;
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

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var pipelines = _serviceProvider.GetServices(pipelineType).Cast<dynamic>().ToList();

            Func<Task<TResponse>> handleRequest = () => handler.Handle((dynamic)request, cancellationToken);

            foreach (var pipeline in pipelines.AsEnumerable().Reverse())
            {
                var currentHandler = handleRequest;
                handleRequest = () => pipeline.Handle((dynamic)request, currentHandler, cancellationToken);
            }

            return await handleRequest();
        }

        public async Task Send(IRequest request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
            dynamic handler = _serviceProvider.GetRequiredService(handlerType);
            await handler.Handle((dynamic)request, cancellationToken);
        }
    }
}

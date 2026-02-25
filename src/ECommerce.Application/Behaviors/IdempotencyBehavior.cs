using ECommerce.Application.Features.Commands;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;

namespace ECommerce.Application.Behaviors
{
    public class IdempotencyBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IIdempotentCommand, IRequest<TResponse>
    {
        private readonly HybridCache _cache;

        public IdempotencyBehavior(HybridCache cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"idem:{typeof(TRequest).Name}:{request.IdempotencyKey}";

            var cachedResponse = await _cache.GetOrCreateAsync(
                cacheKey,
                async entry =>
                {
                    var response = await next();
                    return response;
                },
                cancellationToken: cancellationToken);

            return cachedResponse;
        }
    }
}

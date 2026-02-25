using ECommerce.API.Helper;
using ECommerce.Application.Exceptions;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Hybrid;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;

namespace ECommerce.API.Middleware
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HybridCache _cache;
        private const string HeaderName = "Idempotency-Key";

        public IdempotencyMiddleware(RequestDelegate next, HybridCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the endpoint has the [Idempotent] attribute
            var endpoint = context.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<IdempotentAttribute>();

            if (attribute == null)
            {
                await _next(context);
                return;
            }

            //// Only apply to POST
            //if (!context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            //{
            //    await _next(context);
            //    return;
            //}

            if (!context.Request.Headers.TryGetValue(HeaderName, out var idempotencyKey) || string.IsNullOrWhiteSpace(idempotencyKey))
            {
                //await _next(context);
                //context.Response.StatusCode = StatusCodes.Status400BadRequest;
                throw new BadRequestException("Idempotency-Key header is required for POST requests.");
                //return;
            }

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
            var cacheKey = $"idem:{userId}:{idempotencyKey}";

            //try
            //{
                // Check cache
                var cachedResponse = await _cache.GetOrCreateAsync<CachedResponse?>(
                    cacheKey,
                    async token => await ExecuteAndCaptureResponse(context, token)
                );

                if (cachedResponse != null)
                {
                    context.Response.Headers.Append("X-Idempotency-Replayed", "true");
                    context.Response.StatusCode = cachedResponse.StatusCode;
                    context.Response.ContentType = cachedResponse.ContentType;
                    await context.Response.WriteAsync(cachedResponse.Body);
                    return;
                }
            //}
            //catch (Exception)
            //{
            //}
        }

        private async Task<CachedResponse> ExecuteAndCaptureResponse(HttpContext context, CancellationToken token)
        {
            // Capture the original response body
            var originalBodyStream = context.Response.Body;
            using var memoryStream = new MemoryStream();

            try
            {
                context.Response.Body = memoryStream;

                await _next(context);

                // Restore original stream and write body back
                //memoryStream.Position = 0;
                //await memoryStream.CopyToAsync(originalBodyStream);

                if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
                {
                    // Don't cache non-success responses
                    //throw new Exception("Non-success response, not caching");
                    return null!;
                }

                memoryStream.Position = 0;
                var body = await new StreamReader(memoryStream).ReadToEndAsync(token);

                return new CachedResponse
                {
                    StatusCode = context.Response.StatusCode,
                    ContentType = context.Response.ContentType ?? "application/json",
                    Body = body
                };
            }
            finally
            {
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }
    }
}

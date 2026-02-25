using MediatR;

namespace ECommerce.Application.Features.Commands
{
    public interface IIdempotentCommand //<TResponse> : IRequest<TResponse>
    {
        string IdempotencyKey { get; }
    }
}

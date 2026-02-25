using MediatR;

namespace ECommerce.Application.Interfaces
{
    public interface IIdempotentCommand //<TResponse> : IRequest<TResponse>
    {
        string IdempotencyKey { get; }
    }
}

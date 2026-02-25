using ECommerce.Application.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Queries.Orders.GetOrdersByUser
{
    public class GetOrdersByUserQuery : IRequest<List<OrderDto>>
    {
        public Guid UserId { get; set; }
    }
}

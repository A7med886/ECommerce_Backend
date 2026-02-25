using ECommerce.Application.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Queries.Orders.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderDetailDto>
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
    }
}

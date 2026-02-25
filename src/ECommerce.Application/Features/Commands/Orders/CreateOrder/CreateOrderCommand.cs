using ECommerce.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Orders.CreateOrder
{
    public class CreateOrderCommand : IRequest<CreateOrderResponse>
    {
        public Guid UserId { get; set; }
        public string? DiscountCode { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}

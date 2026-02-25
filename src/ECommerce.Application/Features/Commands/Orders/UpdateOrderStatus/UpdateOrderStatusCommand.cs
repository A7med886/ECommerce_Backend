using MediatR;

namespace ECommerce.Application.Features.Commands.Orders.UpdateOrderStatus
{
    
    public class UpdateOrderStatusCommand : IRequest<Unit>
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;

namespace ECommerce.Application.Features.Commands.Orders.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISignalRService _signalRService;

        public UpdateOrderStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ISignalRService signalRService)
        {
            _unitOfWork = unitOfWork;
            _signalRService = signalRService;
        }

        public async Task<Unit> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            // Get order
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, true, cancellationToken);

            if (order == null)
            {
                throw new NotFoundException("Order not found");
            }

            // Parse and validate status
            if (!Enum.TryParse<OrderStatus>(request.Status, out var newStatus))
            {
                throw new ValidationException("Invalid order status");
            }

            var oldStatus = order.Status;
            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CommitAsync();

            // Send real-time notification
            var statusMessage = GetStatusMessage(newStatus);
            await _signalRService.NotifyOrderStatusChanged(
                order.UserId,
                order.Id,
                newStatus.ToString(),
                statusMessage
            );

            Console.WriteLine($"Order {order.Id} status updated: {oldStatus} → {newStatus}");

            return Unit.Value;
        }

        private string GetStatusMessage(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => "Your order is pending confirmation",
                OrderStatus.Confirmed => "Your order has been confirmed!",
                OrderStatus.Processing => "Your order is being processed",
                OrderStatus.Shipped => "Your order has been shipped!",
                OrderStatus.Delivered => "Your order has been delivered!",
                OrderStatus.Cancelled => "Your order has been cancelled",
                _ => "Order status updated"
            };
        }
    }
}
using AutoMapper;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Orders.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISignalRService _signalRService;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ISignalRService signalRService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _signalRService = signalRService;
        }

        public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Get user
                var user = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(request.UserId, false, cancellationToken);

                if (user == null)
                {
                    throw new NotFoundException("User not found");
                }

                // Get products with locking to prevent concurrent updates
                var productIds = request.Items.Select(x => x.ProductId).ToList();
                var products = await _unitOfWork.Repository<Product>()
                    .GetByConditionAsync(p => productIds.Contains(p.Id) && p.IsActive, true, cancellationToken);

                if (products.Count() != productIds.Count)
                {
                    throw new ValidationException("One or more products not found");
                }

                // Validate stock availability
                foreach (var item in request.Items)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product == null)
                        throw new NotFoundException($"Product {item.ProductId} not found");

                    if (product.Stock < item.Quantity)
                        throw new BadRequestException($"Insufficient stock for {product.Name}. Available: {product.Stock}");
                }

                // Calculate amounts
                decimal subTotal = 0;
                var orderItems = new List<OrderItem>();

                foreach (var item in request.Items)
                {
                    var product = products.First(p => p.Id == item.ProductId);
                    var itemSubtotal = product.Price * item.Quantity;
                    subTotal += itemSubtotal;

                    orderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,
                        Subtotal = itemSubtotal
                    });

                    // Deduct stock
                    product.Stock -= item.Quantity;
                }

                // Apply discount
                decimal discountAmount = 0;
                if (!string.IsNullOrEmpty(request.DiscountCode))
                {
                    discountAmount = CalculateDiscount(request.DiscountCode, subTotal);
                }

                decimal totalAmount = subTotal - discountAmount;

                // Create order
                var order = new Order
                {
                    UserId = request.UserId,
                    SubTotal = subTotal,
                    DiscountAmount = discountAmount,
                    TotalAmount = totalAmount,
                    DiscountCode = request.DiscountCode,
                    Status = OrderStatus.Confirmed,
                    OrderItems = orderItems
                };

                await _unitOfWork.Repository<Order>().AddAsync(order, cancellationToken);

                // Save changes and commit transaction
                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Send SignalR notifications
                Console.WriteLine($"Sending SignalR notifications for order {order.Id}...");

                // 1. Notify customer that order is confirmed
                await _signalRService.NotifyOrderStatusChanged(
                    order.UserId,
                    order.Id,
                    "Confirmed",
                    "Your order has been confirmed!"
                );
                Console.WriteLine($"Sent confirmation to customer UserId: {order.UserId}");

                // 2. Notify admins about new order
                string customerName = $"{user.FirstName} {user.LastName}";
                await _signalRService.NotifyAdminNewOrder(
                    order.Id,
                    customerName,
                    order.TotalAmount,
                    order.UserId
                );
                Console.WriteLine($"Sent new order notification to admins");

                // 3. Notify about stock changes
                foreach (var item in request.Items)
                {
                    var product = products.First(p => p.Id == item.ProductId);
                    await _signalRService.NotifyStockChanged(product.Id, product.Stock);
                    Console.WriteLine($"Sent stock update for product {product.Id}");
                }

                // Return response
                var response = new CreateOrderResponse
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    SubTotal = order.SubTotal,
                    DiscountAmount = order.DiscountAmount,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status.ToString(),
                    Items = orderItems.Select(oi => new OrderItemResponse
                    {
                        ProductId = oi.ProductId,
                        ProductName = products.First(p => p.Id == oi.ProductId).Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Subtotal = oi.Subtotal
                    }).ToList()
                };

                return response;
            }
            catch (DbUpdateConcurrencyException)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new BadRequestException("Stock was modified by another user. Please try again.");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private decimal CalculateDiscount(string discountCode, decimal subTotal)
        {
            // Simple discount logic - enhance as needed
            return discountCode.ToUpper() switch
            {
                "SAVE10" => subTotal * 0.10m,
                "SAVE20" => subTotal * 0.20m,
                "FLAT50" => Math.Min(50, subTotal),
                _ => 0
            };
        }
    }
}

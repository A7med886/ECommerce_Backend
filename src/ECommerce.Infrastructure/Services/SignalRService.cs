using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using ECommerce.Infrastructure.Hubs;

namespace ECommerce.Infrastructure.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly IHubContext<AppHub> _hubContext;

        public SignalRService(IHubContext<AppHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyOrderStatusChanged(Guid userId, Guid orderId, string status, string message)
        {
            var notification = new
            {
                type = "OrderStatusChanged",
                orderId,
                status,
                message,
                timestamp = DateTime.UtcNow
            };

            // ONLY notify the customer (order owner)
            await _hubContext.Clients.Group($"User_{userId}")
                .SendAsync("OrderStatusChanged", notification);

            // Also notify anyone specifically subscribed to this order
            //await _hubContext.Clients.Group($"Order_{orderId}")
            //    .SendAsync("OrderStatusChanged", notification);

            Console.WriteLine($"Order {orderId} status notification sent to User_{userId}");
        }

        public async Task NotifyAdminNewOrder(Guid orderId, string customerName, decimal totalAmount, Guid userId)
        {
            var notification = new
            {
                type = "NewOrder",
                orderId,
                customerName,
                totalAmount,
                userId,
                timestamp = DateTime.UtcNow
            };

            // ✅ ONLY notify admins
            await _hubContext.Clients.Group("Admins")
                .SendAsync("NewOrder", notification);

            Console.WriteLine($"New order {orderId} notification sent to Admins only");
        }

        public async Task NotifyStockChanged(Guid productId, int newStock)
        {
            var notification = new
            {
                type = "StockChanged",
                productId,
                stock = newStock,
                timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"Product_{productId}")
                .SendAsync("StockChanged", notification);

            Console.WriteLine($"Stock changed for product {productId}: {newStock}");
        }

        public async Task NotifyProductPriceChanged(Guid productId, decimal newPrice)
        {
            var notification = new
            {
                type = "PriceChanged",
                productId,
                price = newPrice,
                timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"Product_{productId}")
                .SendAsync("PriceChanged", notification);

            Console.WriteLine($"Price changed for product {productId}: ${newPrice}");
        }

        public async Task SendNotificationToUser(Guid userId, string title, string message, string type)
        {
            var notification = new
            {
                type = "Notification",
                title,
                message,
                notificationType = type,
                timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group($"User_{userId}")
                .SendAsync("Notification", notification);
        }

        public async Task SendNotificationToAdmins(string title, string message, string type)
        {
            var notification = new
            {
                type = "Notification",
                title,
                message,
                notificationType = type,
                timestamp = DateTime.UtcNow
            };

            await _hubContext.Clients.Group("Admins")
                .SendAsync("Notification", notification);
        }
    }
}


//using ECommerce.Application.Interfaces.Services;
//using ECommerce.Infrastructure.Hubs;
//using Microsoft.AspNetCore.SignalR;

//namespace ECommerce.Infrastructure.Services
//{
//    public class SignalRService : ISignalRService
//    {
//        private readonly IHubContext<AppHub> _hubContext;

//        public SignalRService(IHubContext<AppHub> hubContext)
//        {
//            _hubContext = hubContext;
//        }

//        public async Task NotifyOrderStatusChanged(Guid userId, Guid orderId, string status, string message)
//        {
//            var notification = new
//            {
//                type = "OrderStatusChanged",
//                orderId,
//                status,
//                message,
//                timestamp = DateTime.UtcNow
//            };

//            // Notify specific user
//            await _hubContext.Clients.Group($"User_{userId}")
//                .SendAsync("OrderStatusChanged", notification);

//            // Also notify anyone watching this specific order
//            await _hubContext.Clients.Group($"Order_{orderId}")
//                .SendAsync("OrderStatusChanged", notification);

//            Console.WriteLine($"📦 Order {orderId} status changed to {status}");
//        }

//        public async Task NotifyAdminNewOrder(Guid orderId, string customerName, decimal totalAmount, Guid userId)
//        {
//            var notification = new
//            {
//                type = "NewOrder",
//                orderId,
//                customerName,
//                totalAmount,
//                userId,
//                timestamp = DateTime.UtcNow
//            };

//            await _hubContext.Clients.Group("Admins")
//                .SendAsync("NewOrder", notification);

//            Console.WriteLine($"🔔 New order {orderId} notification sent to admins");
//        }

//        public async Task NotifyStockChanged(Guid productId, int newStock)
//        {
//            var notification = new
//            {
//                type = "StockChanged",
//                productId,
//                stock = newStock,
//                timestamp = DateTime.UtcNow
//            };

//            // Notify anyone viewing this product
//            await _hubContext.Clients.Group($"Product_{productId}")
//                .SendAsync("StockChanged", notification);

//            Console.WriteLine($"📊 Stock changed for product {productId}: {newStock}");
//        }

//        public async Task NotifyProductPriceChanged(Guid productId, decimal newPrice)
//        {
//            var notification = new
//            {
//                type = "PriceChanged",
//                productId,
//                price = newPrice,
//                timestamp = DateTime.UtcNow
//            };

//            await _hubContext.Clients.Group($"Product_{productId}")
//                .SendAsync("PriceChanged", notification);

//            Console.WriteLine($"💰 Price changed for product {productId}: ${newPrice}");
//        }

//        public async Task SendNotificationToUser(Guid userId, string title, string message, string type)
//        {
//            var notification = new
//            {
//                type = "Notification",
//                title,
//                message,
//                notificationType = type, // success, info, warning, error
//                timestamp = DateTime.UtcNow
//            };

//            await _hubContext.Clients.Group($"User_{userId}")
//                .SendAsync("Notification", notification);
//        }

//        public async Task SendNotificationToAdmins(string title, string message, string type)
//        {
//            var notification = new
//            {
//                type = "Notification",
//                title,
//                message,
//                notificationType = type,
//                timestamp = DateTime.UtcNow
//            };

//            await _hubContext.Clients.Group("Admins")
//                .SendAsync("Notification", notification);
//        }
//    }
//}

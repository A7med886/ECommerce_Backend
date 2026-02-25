using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ECommerce.Infrastructure.Hubs
{
    [Authorize]
    public class AppHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // Add user to their personal group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");

                // Add admins to admin group
                if (userRole == "Admin")
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                }

                Console.WriteLine($"User {userId} ({userRole}) connected to AppHub");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");

                if (userRole == "Admin")
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
                }

                Console.WriteLine($"User {userId} disconnected from AppHub");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Product subscriptions
        public async Task SubscribeToProduct(Guid productId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Product_{productId}");
            Console.WriteLine($"Subscribed to Product_{productId}");
        }

        public async Task UnsubscribeFromProduct(Guid productId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Product_{productId}");
            Console.WriteLine($"Unsubscribed from Product_{productId}");
        }

        // Order subscriptions
        public async Task SubscribeToOrder(Guid orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderId}");
            Console.WriteLine($"Subscribed to Order_{orderId}");
        }

        public async Task UnsubscribeFromOrder(Guid orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order_{orderId}");
            Console.WriteLine($"Unsubscribed from Order_{orderId}");
        }

        // Global notifications subscription
        public async Task SubscribeToNotifications()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AllNotifications");
        }
    }
}
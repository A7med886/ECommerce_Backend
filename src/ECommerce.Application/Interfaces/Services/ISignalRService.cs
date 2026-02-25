
namespace ECommerce.Application.Interfaces.Services
{
    public interface ISignalRService
    {
        // Order notifications
        Task NotifyOrderStatusChanged(Guid userId, Guid orderId, string status, string message);
        Task NotifyAdminNewOrder(Guid orderId, string customerName, decimal totalAmount, Guid userId);

        // Product notifications
        Task NotifyStockChanged(Guid productId, int newStock);
        Task NotifyProductPriceChanged(Guid productId, decimal newPrice);

        // General notifications
        Task SendNotificationToUser(Guid userId, string title, string message, string type);
        Task SendNotificationToAdmins(string title, string message, string type);
    }
}

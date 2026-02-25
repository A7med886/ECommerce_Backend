namespace ECommerce.Application.DTOs
{
    public class OrderDetailDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? DiscountCode { get; set; }
        public List<OrderItemDetailDto> Items { get; set; } = new();
    }
}

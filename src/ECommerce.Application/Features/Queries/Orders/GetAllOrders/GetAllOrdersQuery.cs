using ECommerce.Application.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Queries.Orders.GetAllOrders
{
    public class GetAllOrdersQuery : IRequest<PagedResult<OrderQueryDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Status { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class OrderQueryDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    //public class PagedResult<T>
    //{
    //    public List<T> Items { get; set; } = new();
    //    public int TotalItems { get; set; }
    //    public int PageNumber { get; set; }
    //    public int PageSize { get; set; }
    //    public int TotalPages { get; set; }
    //    public bool HasPreviousPage { get; set; }
    //    public bool HasNextPage { get; set; }
    //}
}
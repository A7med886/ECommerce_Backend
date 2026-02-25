using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Queries.Orders.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, PagedResult<OrderQueryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<OrderQueryDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Order>().GetQueryable()
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<OrderStatus>(request.Status, out var status))
            {
                query = query.Where(o => o.Status == status);
            }

            // Search by customer name or email
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm;
                query = query.Where(o =>
                    o.User.Email.Contains(searchTerm) ||
                    o.User.FirstName.Contains(searchTerm) ||
                    o.User.LastName.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OrderQueryDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    CustomerName = $"{o.User.FirstName} {o.User.LastName}",
                    CustomerEmail = o.User.Email,
                    OrderDate = o.OrderDate,
                    SubTotal = o.SubTotal,
                    DiscountAmount = o.DiscountAmount,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    ItemCount = o.OrderItems.Count
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<OrderQueryDto>
            {
                Items = orders,
                TotalItems = totalItems,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
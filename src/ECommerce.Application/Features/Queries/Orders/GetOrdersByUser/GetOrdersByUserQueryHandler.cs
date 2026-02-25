using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Features.Queries.Orders.GetOrdersByUser
{
    public class GetOrdersByUserQueryHandler : IRequestHandler<GetOrdersByUserQuery, List<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrdersByUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<OrderDto>> Handle(GetOrdersByUserQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.Repository<Order>()
                .GetQueryable()
                .AsNoTracking()
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == request.UserId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<OrderDto>>(orders);
        }
    }
}

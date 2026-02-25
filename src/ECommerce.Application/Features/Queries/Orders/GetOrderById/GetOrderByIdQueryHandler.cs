using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Features.Queries.Orders.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderDetailDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Repository<Order>()
                .GetQueryable()
                .AsNoTracking()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
                throw new NotFoundException($"Order with ID {request.OrderId} not found.");

            ////Customers can only see their own orders
            //if (order.UserId != request.UserId)
            //    throw new UnauthorizedException("You are not authorized to view this order.");

            return _mapper.Map<OrderDetailDto>(order);
        }
    }
}

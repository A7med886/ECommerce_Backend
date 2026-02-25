using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Features.Queries.Products.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Product>()
                .GetQueryable()
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();

            if (!request.IsAdmin)
                query = query.Where(p => p.IsActive);

            var product = await query.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
            if (product == null)
                throw new NotFoundException($"Product with ID {request.ProductId} not found.");

            return _mapper.Map<ProductDto>(product);
        }
    }
}

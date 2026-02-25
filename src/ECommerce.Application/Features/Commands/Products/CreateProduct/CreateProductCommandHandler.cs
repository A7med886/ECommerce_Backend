using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Features.Commands.Products.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Repository<Category>()
                .GetByIdAsync(request.CategoryId, false, cancellationToken);

            if (category == null)
                throw new NotFoundException($"Category with ID {request.CategoryId} not found.");

            var product = new Product
            {
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId,
                ImageUrl = request.ImageUrl,
                IsActive = true
            };

            await _unitOfWork.Repository<Product>().AddAsync(product, cancellationToken);
            await _unitOfWork.CommitAsync();

            //product.Category = category;
            //return _mapper.Map<ProductDto>(product);
            var productDto = _mapper.Map<ProductDto>(product);
            productDto.CategoryName = category.Name;
            return productDto;
        }
    }
}

using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Features.Commands.Products.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Repository<Product>()
                .GetByIdAsync(request.Id, true, cancellationToken);

            if (product == null)
                throw new NotFoundException($"Product with ID {request.Id} not found.");

            var category = await _unitOfWork.Repository<Category>()
                .GetByIdAsync(request.CategoryId, false, cancellationToken);

            if (category == null)
                throw new NotFoundException($"Category with ID {request.CategoryId} not found.");

            product.Name = request.Name.Trim();
            product.Description = request.Description.Trim();
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.CategoryId = request.CategoryId;
            product.ImageUrl = request.ImageUrl;
            product.IsActive = request.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CommitAsync();

            //product.Category = category;
            //return _mapper.Map<ProductDto>(product);
            var productDto = _mapper.Map<ProductDto>(product);
            productDto.CategoryName = category.Name;
            return productDto;
        }
    }
}

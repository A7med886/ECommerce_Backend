using ECommerce.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Features.Commands.Products.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id, true, cancellationToken);

            if (product == null)
                throw new NotFoundException($"Product with ID {request.Id} not found.");

            //_unitOfWork.Repository<Product>().Delete(product, cancellationToken);
            product.IsActive = false; // Soft delete
            await _unitOfWork.CommitAsync();

            return 1;
        }
    }
}

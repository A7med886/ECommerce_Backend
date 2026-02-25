using ECommerce.Application.DTOs;
using MediatR;

namespace ECommerce.Application.Features.Queries.Products.GetProductById
{
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        public Guid ProductId { get; set; }
        public bool IsAdmin { get; set; }
    }
}

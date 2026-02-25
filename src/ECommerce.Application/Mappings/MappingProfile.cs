using AutoMapper;
using ECommerce.Application.DTOs;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Product => ProductDto
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));

            // Category => CategoryDto
            CreateMap<Category, CategoryDto>();

            // Order => OrderDto (summary)
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ItemCount,
                    opt => opt.MapFrom(src => src.OrderItems.Count));

            // Order => OrderDetailDto (full detail)
            CreateMap<Order, OrderDetailDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Items,
                    opt => opt.MapFrom(src => src.OrderItems));

            // OrderItem => OrderItemDetailDto
            CreateMap<OrderItem, OrderItemDetailDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));
        }
    }
}

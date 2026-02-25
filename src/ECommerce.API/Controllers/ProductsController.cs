using ECommerce.API.Helper;
using ECommerce.Application.DTOs;
using ECommerce.Application.Features.Commands.Products.CreateProduct;
using ECommerce.Application.Features.Commands.Products.DeleteProduct;
using ECommerce.Application.Features.Commands.Products.UpdateProduct;
using ECommerce.Application.Features.Queries.Products.GetProductById;
using ECommerce.Application.Features.Queries.Products.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
            [FromQuery] GetProductsQuery query)
        {
            query.IsAdmin = User.IsInRole("Admin");
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
        {
            var query = new GetProductByIdQuery { ProductId = id, IsAdmin = User.IsInRole("Admin") };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Idempotent]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct(
            [FromBody] CreateProductCommand command)
        {
            //var idemKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
            //Console.WriteLine($"Idempotency-Key: {idemKey}");
            //return Conflict($"Idempotency-Key: {idemKey}");

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(
            Guid id, [FromBody] UpdateProductCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> DeleteProduct(
            Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand { Id = id });
            return Ok(result);
        }
    }
}

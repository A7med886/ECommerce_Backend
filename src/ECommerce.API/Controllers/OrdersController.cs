using ECommerce.API.Helper;
using ECommerce.Application.DTOs;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Features.Commands.Orders.CreateOrder;
using ECommerce.Application.Features.Commands.Orders.UpdateOrderStatus;
using ECommerce.Application.Features.Queries.Orders.GetAllOrders;
using ECommerce.Application.Features.Queries.Orders.GetOrderById;
using ECommerce.Application.Features.Queries.Orders.GetOrdersByUser;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<OrderQueryDto>>> GetAllOrders([FromQuery] GetAllOrdersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPatch("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateOrderStatus(Guid orderId, [FromBody] UpdateOrderStatusCommand command)
        {
            command.OrderId = orderId;
            await _mediator.Send(command);
            return Ok(new { message = "Order status updated successfully" });
        }

        [Idempotent]
        [HttpPost]
        public async Task<ActionResult<CreateOrderResponse>> CreateOrder([FromBody] CreateOrderCommand command)
        {
            //var idemKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
            //Console.WriteLine($"Idempotency-Key: {idemKey}");
            //return Conflict($"Idempotency-Key: {idemKey}");

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            command.UserId = userId;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("user")]
        public async Task<ActionResult<List<OrderDto>>> GetMyOrders()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var query = new GetOrdersByUserQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailDto>> GetOrderById(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var query = new GetOrderByIdQuery { OrderId = id, UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}

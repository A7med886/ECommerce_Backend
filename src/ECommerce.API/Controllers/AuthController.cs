using ECommerce.API.Helper;
using ECommerce.Application.Features.Commands.Auth.Login;
using ECommerce.Application.Features.Commands.Auth.RefreshTokenCommand;
using ECommerce.Application.Features.Commands.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginCommand command)
        {
            command.IpAddress = GetIpAddress();
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Idempotent]
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register(
            [FromBody] RegisterCommand command)
        {
            //var idemKey = Request.Headers["Idempotency-Key"].FirstOrDefault();
            //Console.WriteLine($"Idempotency-Key: {idemKey}");
            //return Conflict($"Idempotency-Key: {idemKey}");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            command.IpAddress = GetIpAddress();
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"].ToString();

            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";
        }
    }
}

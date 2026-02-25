using MediatR;

namespace ECommerce.Application.Features.Commands.Auth.Register
{
    public class RegisterCommand : IRequest<RegisterResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}

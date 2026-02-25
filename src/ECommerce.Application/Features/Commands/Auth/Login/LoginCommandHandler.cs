using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace ECommerce.Application.Features.Commands.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Email == request.Email, false, cancellationToken);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedException("Account is inactive");
            }

            // Generate tokens
            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Store refresh token in database
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = _jwtService.GetRefreshTokenExpiryDate(),
                CreatedByIp = request.IpAddress ?? "Unknown"
            };

            await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshTokenEntity, cancellationToken);
            await _unitOfWork.CommitAsync();

            // Remove old/expired refresh tokens for this user
            await RemoveOldRefreshTokens(user.Id, cancellationToken);

            return new LoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString()
            };
        }

        private async Task RemoveOldRefreshTokens(Guid userId, CancellationToken cancellationToken)
        {
            // Remove refresh tokens older than refresh token TTL
            var expiredTokens = await _unitOfWork.Repository<RefreshToken>()
                .GetByConditionAsync(rt =>
                    rt.UserId == userId &&
                    (rt.IsRevoked || rt.ExpiresAt < DateTime.UtcNow.AddDays(-_jwtService.GetRefreshTokenExpiryDays())), false, cancellationToken);

            foreach (var token in expiredTokens)
            {
                _unitOfWork.Repository<RefreshToken>().Delete(token);
            }

            await _unitOfWork.CommitAsync();
        }
    }
}

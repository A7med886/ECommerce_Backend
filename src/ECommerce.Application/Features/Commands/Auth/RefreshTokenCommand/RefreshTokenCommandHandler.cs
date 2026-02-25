using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using MediatR;
using System.Security.Claims;

namespace ECommerce.Application.Features.Commands.Auth.RefreshTokenCommand
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public RefreshTokenCommandHandler(
            IUnitOfWork unitOfWork,
            IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Validate the access token (even if expired)
            var principal = _jwtService.ValidateToken(request.Token);
            if (principal == null)
            {
                throw new UnauthorizedException("Invalid token");
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedException("Invalid token claims");
            }

            var userId = Guid.Parse(userIdClaim);

            // Get the refresh token from database
            var refreshToken = await _unitOfWork.Repository<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.UserId == userId, true, cancellationToken);

            if (refreshToken == null)
            {
                throw new UnauthorizedException("Invalid refresh token");
            }

            // Check if refresh token is active
            if (!refreshToken.IsActive)
            {
                throw new UnauthorizedException("Refresh token is no longer active");
            }

            // Get user
            var user = await _unitOfWork.Repository<User>()
                .GetByIdAsync(userId, false, cancellationToken);

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedException("User not found or inactive");
            }

            // Generate new tokens
            var newAccessToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Revoke old refresh token
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = request.IpAddress ?? "Unknown";
            refreshToken.ReplacedByToken = newRefreshToken;
            _unitOfWork.Repository<RefreshToken>().Update(refreshToken);

            // Create new refresh token
            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = _jwtService.GetRefreshTokenExpiryDate(),
                CreatedByIp = request.IpAddress ?? "Unknown"
            };

            await _unitOfWork.Repository<RefreshToken>().AddAsync(newRefreshTokenEntity, cancellationToken);
            await _unitOfWork.CommitAsync();

            return new RefreshTokenResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString()
            };
        }
    }
}
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        DateTime GetRefreshTokenExpiryDate();
        int GetRefreshTokenExpiryDays();
        System.Security.Claims.ClaimsPrincipal? ValidateToken(string token);
    }
}

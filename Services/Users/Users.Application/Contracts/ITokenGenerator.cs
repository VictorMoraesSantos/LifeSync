using BuildingBlocks.Results;
using System.Security.Claims;

namespace Users.Application.Interfaces
{
    public interface ITokenGenerator
    {
        Result<string> GenerateToken(string userId, string email, IList<string> roles, CancellationToken cancellationToken, IList<Claim>? extraClaims = null);
        Result<string> GenerateRefreshToken();
        Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token);
    }
}

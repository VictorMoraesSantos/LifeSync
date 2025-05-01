using System.Security.Claims;

namespace Users.Application.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateTokenAsync(string userId, string email, IList<string> roles, CancellationToken cancellationToken);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}

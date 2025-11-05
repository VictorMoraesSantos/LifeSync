using BuildingBlocks.Results;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Users.Application.Interfaces;
using Users.Infrastructure.Settings;

namespace Users.Infrastructure.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public TokenGenerator(IOptions<JwtSettings> jwtSettings)
        {
            _key = jwtSettings.Value.Key;
            _issuer = jwtSettings.Value.Issuer;
            _audience = jwtSettings.Value.Audience;
            _expiryMinutes = jwtSettings.Value.ExpiryMinutes;
        }

        public Result<string> GenerateToken(string userId, string email, IList<string> roles, CancellationToken cancellationToken, IList<Claim>? extraClaims = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim("UserId", userId)
            };

            // profile claims
            if (extraClaims is not null)
            {
                foreach (var c in extraClaims)
                {
                    // avoid duplicates
                    if (!claims.Any(x => x.Type == c.Type))
                        claims.Add(c);
                }
            }

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_expiryMinutes)),
                signingCredentials: signingCredentials);
            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Result.Success(encodedToken);
        }

        public Result<string> GenerateRefreshToken()
        {

            byte[] randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var refreshToken = Convert.ToBase64String(randomNumber);

            return Result.Success(refreshToken);
        }

        public Result<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                ValidateLifetime = false,
                ValidIssuer = _issuer,
                ValidAudience = _audience
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Token inválido");
            }

            return Result.Success(principal);
        }
    }
}

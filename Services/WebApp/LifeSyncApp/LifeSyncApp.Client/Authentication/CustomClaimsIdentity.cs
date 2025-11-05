using System.Security.Claims;

namespace LifeSyncApp.Client.Authentication;

public class CustomClaimsIdentity : ClaimsIdentity
{
    public CustomClaimsIdentity(IEnumerable<Claim> claims, string? authenticationType, string? nameType, string? roleType)
        : base(claims, authenticationType, nameType, roleType)
    {
    }

    public string? UserId => FindFirst(ClaimTypes.NameIdentifier)?.Value ?? FindFirst("UserId")?.Value;
    public string? Email => FindFirst(ClaimTypes.Email)?.Value;
    public string? FirstName => FindFirst(ClaimTypes.GivenName)?.Value;
    public string? LastName => FindFirst(ClaimTypes.Surname)?.Value;
}
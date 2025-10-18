using Users.Application.DTOs.User;

namespace Users.Application.DTOs.Auth
{
    public record AuthResult(string AccessToken, string RefreshToken, UserDTO User);
}

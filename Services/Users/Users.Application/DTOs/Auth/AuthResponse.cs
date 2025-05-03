using Users.Application.DTOs.User;

namespace Users.Application.DTOs.Auth
{
    public record AuthResponse(string AccessToken, string RefreshToken, UserDTO User);
}

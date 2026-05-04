namespace LifeSyncApp.Models.Auth
{
    public record AuthResult(string AccessToken, string RefreshToken, UserDTO User);
}

namespace LifeSyncApp.Client.Models;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    // Client-side only
    public string ConfirmPassword { get; set; } = string.Empty;
}

public record AuthResult(string AccessToken, string RefreshToken, UserDTO User);

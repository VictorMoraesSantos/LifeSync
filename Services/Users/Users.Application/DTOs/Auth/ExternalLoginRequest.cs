namespace Users.Application.DTOs.Auth
{
    public record ExternalLoginRequest(string IdToken, string Provider);
}

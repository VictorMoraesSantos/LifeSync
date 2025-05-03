namespace Users.Application.DTOs.Auth
{
    public record SignUpRequest(
        string FirstName,
        string LastName,
        string Email,
        string Password);
}

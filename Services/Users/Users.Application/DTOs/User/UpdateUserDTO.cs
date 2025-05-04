namespace Users.Application.DTOs.User
{
    public record UpdateUserDTO(
        string Id,
        string FirstName,
        string LastName,
        string Email,
        DateOnly? BirthDate);
}

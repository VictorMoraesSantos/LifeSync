namespace Users.Application.DTOs.User
{
    public record UpdateUserDTO(
        int Id,
        string FirstName,
        string LastName,
        string Email,
        DateOnly? BirthDate);
}

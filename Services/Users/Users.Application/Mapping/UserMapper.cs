using Users.Application.DTOs.User;
using Users.Domain.Entities;
using Users.Domain.ValueObjects;

namespace Users.Application.Mapping
{
    public static class UserMapper
    {
        public static UserDTO ToDto(User user)
        {
            UserDTO dto = new UserDTO(
                user.Id.ToString(),
                user.Name.FirstName,
                user.Name.LastName,
                user.Name.FullName,
                user.Email,
                user.BirthDate,
                user.CreatedAt,
                user.LastLoginAt,
                user.IsActive,
                new List<string>()
            );
            return dto;
        }

        public static User ToDomain(UserDTO dto)
        {
            Name name = new Name(dto.FirstName, dto.LastName);
            Contact contact = new Contact(dto.Email);
            User user = new User(name, contact);
            return user;
        }
    }
}

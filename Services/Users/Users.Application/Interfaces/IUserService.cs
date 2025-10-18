using BuildingBlocks.Results;
using System.Security.Claims;
using Users.Application.DTOs.User;

namespace Users.Application.Interfaces
{
    public interface IUserService
    {
        // Operações do usuário autenticado
        Task<Result<UserDTO>> GetCurrentUserDetailsAsync(ClaimsPrincipal user);
        Task<Result<bool>> UpdateCurrentUserProfileAsync(ClaimsPrincipal user, string firstName, string lastName, string email);
        Task<Result<bool>> ChangeCurrentUserPasswordAsync(ClaimsPrincipal user, string currentPassword, string newPassword);
        Task<Result<bool>> DeleteCurrentUserAsync(ClaimsPrincipal user);

        // Operações administrativas
        Task<Result<UserDTO>> GetUserDetailsAsync(string userId);
        Task<Result<IList<UserDTO>>> GetAllUsersAsync();
        Task<Result<IList<UserDTO>>> GetAllUsersDetailsAsync();
        Task<Result<bool>> IsUserEmailUniqueAsync(string email);
        Task<Result<bool>> UpdateUserProfileAsync(UpdateUserDTO dto);
        Task<Result<bool>> DeleteUserAsync(string userId);
    }
}

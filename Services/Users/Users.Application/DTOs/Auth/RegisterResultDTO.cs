using Users.Application.DTOs.User;

namespace Users.Application.DTOs.Auth
{
    public class RegisterResultDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserSummaryDTO User { get; set; }
    }
}

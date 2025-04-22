namespace Users.Application.DTOs.Auth
{
    public class ConfirmEmailDTO
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}

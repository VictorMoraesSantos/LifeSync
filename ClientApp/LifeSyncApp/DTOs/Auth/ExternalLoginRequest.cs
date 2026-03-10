namespace LifeSyncApp.DTOs.Auth
{
    public class ExternalLoginRequest
    {
        public string IdToken { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
    }
}

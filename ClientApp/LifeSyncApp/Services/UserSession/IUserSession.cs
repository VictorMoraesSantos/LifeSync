namespace LifeSyncApp.Services.UserSession
{
    public interface IUserSession
    {
        int UserId { get; }
        Task InitializeAsync();
    }
}

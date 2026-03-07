namespace LifeSyncApp.Services.UserSession
{
    public class UserSession : IUserSession
    {
        private int _userId;

        public int UserId => _userId;

        public async Task InitializeAsync()
        {
            try
            {
                var userIdStr = await SecureStorage.GetAsync("user_id");
                _userId = int.TryParse(userIdStr, out var id) ? id : 0;
            }
            catch
            {
                _userId = 0;
            }
        }
    }
}

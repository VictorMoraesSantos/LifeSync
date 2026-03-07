using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.UserSession;

namespace LifeSyncApp
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;
        private readonly IUserSession _userSession;

        public App(IAuthService authService, IUserSession userSession)
        {
            InitializeComponent();
            _authService = authService;
            _userSession = userSession;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var shell = new AppShell();
            var window = new Window(shell);

            shell.Navigated += OnShellNavigated;

            return window;
        }

        private async void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
        {
            if (sender is Shell shell)
            {
                shell.Navigated -= OnShellNavigated;
            }

            try
            {
                var isAuthenticated = await _authService.IsAuthenticatedAsync();
                if (isAuthenticated)
                {
                    await _userSession.InitializeAsync();
                    await Shell.Current.GoToAsync("//MainPage");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Auth check failed: {ex.Message}");
            }
        }
    }
}

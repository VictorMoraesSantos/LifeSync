using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeSyncApp.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty] private bool _isBusy;

        [ObservableProperty] private string _title = string.Empty;

        protected const int DefaultCacheMinutes = 5;

        protected static bool IsCacheExpired(DateTime? timestamp, int minutes = DefaultCacheMinutes)
            => timestamp == null || (DateTime.Now - timestamp.Value).TotalMinutes >= minutes;

        protected static Task ShowAlertAsync(string title, string message, string cancel = "OK")
        {
            if (Shell.Current?.CurrentPage is Page page)
                return page.DisplayAlert(title, message, cancel);
            return Task.CompletedTask;
        }

        protected static Task<bool> ShowConfirmAsync(string title, string message, string accept = "Sim", string cancel = "Não")
        {
            if (Shell.Current?.CurrentPage is Page page)
                return page.DisplayAlert(title, message, accept, cancel);
            return Task.FromResult(false);
        }

        protected static Task<string> ShowPromptAsync(string title, string message, string accept = "OK", string cancel = "Cancelar", string? placeholder = null, int maxLength = -1, Keyboard? keyboard = null, string initialValue = "")
        {
            if (Shell.Current?.CurrentPage is Page page)
                return page.DisplayPromptAsync(title, message, accept, cancel, placeholder, maxLength, keyboard, initialValue);
            return Task.FromResult(string.Empty);
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeSyncApp.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        protected const int DefaultCacheMinutes = 5;

        protected static bool IsCacheExpired(DateTime? timestamp, int minutes = DefaultCacheMinutes)
            => timestamp == null || (DateTime.Now - timestamp.Value).TotalMinutes >= minutes;
    }
}

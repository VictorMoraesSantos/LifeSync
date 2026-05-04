using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeSyncApp.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        protected const int DefaultCacheMinutes = 5;

        protected static bool IsCacheExpired(DateTime? timestamp, int minutes = DefaultCacheMinutes)
            => timestamp == null || (DateTime.Now - timestamp.Value).TotalMinutes >= minutes;
    }
}

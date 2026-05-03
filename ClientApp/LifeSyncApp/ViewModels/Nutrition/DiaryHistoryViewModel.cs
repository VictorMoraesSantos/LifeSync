using LifeSyncApp.Constants;
using LifeSyncApp.DTOs.Nutrition.Diary;
using LifeSyncApp.Services.Nutrition;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class DiaryHistoryViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;
        private readonly IUserSession _userSession;

        private string _dateFromText = string.Empty;
        private string _dateToText = string.Empty;
        private int _currentPage = 1;
        private int _totalPages = 1;

        public ObservableCollection<DiaryDTO> Diaries { get; } = new();

        public string DateFromText
        {
            get => _dateFromText;
            set => SetProperty(ref _dateFromText, value);
        }

        public string DateToText
        {
            get => _dateToText;
            set => SetProperty(ref _dateToText, value);
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                SetProperty(ref _currentPage, value);
                OnPropertyChanged(nameof(PageInfo));
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                SetProperty(ref _totalPages, value);
                OnPropertyChanged(nameof(PageInfo));
            }
        }

        public string PageInfo => $"Página {CurrentPage} de {TotalPages}";

        public ICommand GoBackCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand OpenCreateDiaryCommand { get; }
        public ICommand OpenDiaryDetailCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public DiaryHistoryViewModel(INutritionService nutritionService, IUserSession userSession)
        {
            _nutritionService = nutritionService;
            _userSession = userSession;
            Title = "Histórico";

            // Default filter: last 30 days
            var today = DateOnly.FromDateTime(DateTime.Today);
            DateFromText = today.AddDays(-30).ToString("dd/MM/yyyy");
            DateToText = today.ToString("dd/MM/yyyy");

            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            SearchCommand = new Command(async () => { CurrentPage = 1; await LoadDiariesAsync(); });
            OpenCreateDiaryCommand = new Command(async () => await Shell.Current.GoToAsync(AppRoutes.CreateDiaryModal));
            OpenDiaryDetailCommand = new Command<DiaryDTO>(async (d) => await OpenDiaryDetailAsync(d));
            NextPageCommand = new Command(async () => { if (CurrentPage < TotalPages) { CurrentPage++; await LoadDiariesAsync(); } });
            PreviousPageCommand = new Command(async () => { if (CurrentPage > 1) { CurrentPage--; await LoadDiariesAsync(); } });
        }

        public async Task InitializeAsync()
        {
            await LoadDiariesAsync();
        }

        private async Task LoadDiariesAsync()
        {
            try
            {
                IsBusy = true;

                DateOnly? dateFrom = null;
                DateOnly? dateTo = null;

                if (DateOnly.TryParseExact(DateFromText, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var df))
                    dateFrom = df;
                if (DateOnly.TryParseExact(DateToText, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
                    dateTo = dt;

                var diaries = await _nutritionService.SearchDiariesAsync(
                    _userSession.UserId, dateFrom, dateTo, CurrentPage, 5);

                Diaries.Clear();
                foreach (var d in diaries)
                    Diaries.Add(d);

                // Estimate total pages (if fewer results than page size, it's the last page)
                TotalPages = diaries.Count < 5 ? CurrentPage : CurrentPage + 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading diaries: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OpenDiaryDetailAsync(DiaryDTO? diary)
        {
            if (diary == null) return;
            await Shell.Current.GoToAsync(AppRoutes.DiaryDetailPage, new Dictionary<string, object>
            {
                { "Diary", diary }
            });
        }
    }
}

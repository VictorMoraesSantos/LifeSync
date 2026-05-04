using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Constants;
using LifeSyncApp.Models.Nutrition.Diary;
using LifeSyncApp.Services.Nutrition;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public partial class DiaryHistoryViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;
        private readonly IUserSession _userSession;

        [ObservableProperty]
        private string _dateFromText = string.Empty;

        [ObservableProperty]
        private string _dateToText = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PageInfo))]
        private int _currentPage = 1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PageInfo))]
        private int _totalPages = 1;

        public ObservableCollection<DiaryDTO> Diaries { get; } = new();

        public string PageInfo => $"Página {CurrentPage} de {TotalPages}";

        public DiaryHistoryViewModel(INutritionService nutritionService, IUserSession userSession)
        {
            _nutritionService = nutritionService;
            _userSession = userSession;
            Title = "Histórico";

            var today = DateOnly.FromDateTime(DateTime.Today);
            DateFromText = today.AddDays(-30).ToString("dd/MM/yyyy");
            DateToText = today.ToString("dd/MM/yyyy");
        }

        public async Task InitializeAsync()
        {
            await LoadDiariesAsync();
        }

        [RelayCommand]
        private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        private async Task SearchAsync()
        {
            CurrentPage = 1;
            await LoadDiariesAsync();
        }

        [RelayCommand]
        private async Task OpenCreateDiaryAsync() => await Shell.Current.GoToAsync(AppRoutes.CreateDiaryModal);

        [RelayCommand]
        private async Task OpenDiaryDetailAsync(DiaryDTO? diary)
        {
            if (diary == null) return;
            await Shell.Current.GoToAsync(AppRoutes.DiaryDetailPage, new Dictionary<string, object>
            {
                { "Diary", diary }
            });
        }

        [RelayCommand]
        private async Task NextPageAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await LoadDiariesAsync();
            }
        }

        [RelayCommand]
        private async Task PreviousPageAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await LoadDiariesAsync();
            }
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
    }
}

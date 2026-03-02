using LifeSyncApp.DTOs.Nutrition.Diary;
using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

[QueryProperty(nameof(Diary), "Diary")]
public partial class DiaryDetailPage : ContentPage
{
    private readonly DiaryDetailViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;

    public DiaryDTO? Diary { get; set; }

    public DiaryDetailPage(DiaryDetailViewModel viewModel, NutritionViewModel nutritionViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _nutritionViewModel = nutritionViewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.DiaryDeleted += OnDiaryDeleted;
        if (Diary != null)
            _viewModel.Diary = Diary;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.DiaryDeleted -= OnDiaryDeleted;
        _nutritionViewModel.InvalidateDataCache();
    }

    private void OnDiaryDeleted(object? sender, EventArgs e)
    {
        _nutritionViewModel.InvalidateDataCache();
    }
}

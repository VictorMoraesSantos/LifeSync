using LifeSyncApp.DTOs.Nutrition.Diary;
using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

public partial class DiaryDetailPage : ContentPage, IQueryAttributable
{
    private readonly DiaryDetailViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;

    private DiaryDTO? _diary;

    public DiaryDetailPage(DiaryDetailViewModel viewModel, NutritionViewModel nutritionViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _nutritionViewModel = nutritionViewModel;
        BindingContext = _viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Diary", out var value) && value is DiaryDTO dto)
            _diary = dto;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.DiaryDeleted += OnDiaryDeleted;
        if (_diary != null)
            _viewModel.Diary = _diary;
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

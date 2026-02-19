using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

[QueryProperty(nameof(DiaryId), "DiaryId")]
[QueryProperty(nameof(Liquid), "Liquid")]
public partial class ManageLiquidModal : ContentPage
{
    private readonly ManageLiquidViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;

    public int DiaryId { get; set; }
    public LiquidDTO? Liquid { get; set; }

    public ManageLiquidModal(ManageLiquidViewModel viewModel, NutritionViewModel nutritionViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _nutritionViewModel = nutritionViewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnSaved += OnSaved;
        _viewModel.OnCancelled += OnCancelled;
        _viewModel.Initialize(DiaryId, Liquid);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnSaved -= OnSaved;
        _viewModel.OnCancelled -= OnCancelled;
    }

    private async void OnSaved(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
        _nutritionViewModel.InvalidateDataCache();
        await _nutritionViewModel.LoadDataAsync(forceRefresh: true);
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}

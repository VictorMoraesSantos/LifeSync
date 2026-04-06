using CommunityToolkit.Mvvm.Messaging;
using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.Messages;
using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

[QueryProperty(nameof(Meal), "Meal")]
public partial class MealDetailPage : ContentPage
{
    private readonly MealDetailViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;
    private CancellationTokenSource? _skeletonAnimationCts;

    public MealDTO? Meal { get; set; }

    public MealDetailPage(MealDetailViewModel viewModel, NutritionViewModel nutritionViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _nutritionViewModel = nutritionViewModel;
        BindingContext = _viewModel;

        WeakReferenceMessenger.Default.Register<MealFoodChangedMessage>(this, async (r, m) =>
        {
            await _viewModel.RefreshMealAsync();
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.IsLoadingMeal)
            StartSkeletonAnimation();

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        _viewModel.MealDeleted += OnMealDeleted;

        if (Meal != null)
        {
            _viewModel.Meal = Meal;
            Meal = null;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _viewModel.MealDeleted -= OnMealDeleted;
        StopSkeletonAnimation();
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MealDetailViewModel.IsLoadingMeal))
        {
            if (_viewModel.IsLoadingMeal)
                StartSkeletonAnimation();
            else
                StopSkeletonAnimation();
        }
    }

    private void OnMealDeleted(object? sender, EventArgs e)
    {
        _nutritionViewModel.InvalidateDataCache();
    }

    private void StartSkeletonAnimation()
    {
        StopSkeletonAnimation();
        _skeletonAnimationCts = new CancellationTokenSource();
        var token = _skeletonAnimationCts.Token;

        Dispatcher.Dispatch(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await SkeletonContainer.FadeTo(0.4, 800, Easing.SinInOut);
                if (token.IsCancellationRequested) break;
                await SkeletonContainer.FadeTo(1.0, 800, Easing.SinInOut);
            }
        });
    }

    private void StopSkeletonAnimation()
    {
        _skeletonAnimationCts?.Cancel();
        _skeletonAnimationCts?.Dispose();
        _skeletonAnimationCts = null;
    }

    ~MealDetailPage()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}

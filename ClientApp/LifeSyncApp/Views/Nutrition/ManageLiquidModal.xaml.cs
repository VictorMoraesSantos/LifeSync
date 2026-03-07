using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.ViewModels.Nutrition;
using System.ComponentModel;

namespace LifeSyncApp.Views.Nutrition;

public partial class ManageLiquidModal : ContentPage, IQueryAttributable
{
    private readonly ManageLiquidViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;
    private Dictionary<string, (Border Border, Label Label)> _quickButtons = new();

    private int _diaryId;
    private LiquidDTO? _liquid;

    public ManageLiquidModal(ManageLiquidViewModel viewModel, NutritionViewModel nutritionViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _nutritionViewModel = nutritionViewModel;
        BindingContext = _viewModel;

        _quickButtons = new Dictionary<string, (Border, Label)>
        {
            { "100", (Btn100, Lbl100) },
            { "250", (Btn250, Lbl250) },
            { "300", (Btn300, Lbl300) },
            { "500", (Btn500, Lbl500) },
            { "1000", (Btn1000, Lbl1000) },
            { "200", (Btn200, Lbl200) },
        };
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("DiaryId", out var diaryIdValue))
        {
            if (diaryIdValue is int id)
                _diaryId = id;
            else if (diaryIdValue is string s && int.TryParse(s, out var parsed))
                _diaryId = parsed;
        }

        if (query.TryGetValue("Liquid", out var liquidValue) && liquidValue is LiquidDTO dto)
            _liquid = dto;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnSaved += OnSaved;
        _viewModel.OnCancelled += OnCancelled;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        await _viewModel.InitializeAsync(_diaryId, _liquid);
        UpdateButtonStyles(_viewModel.SelectedQuickQuantity);

        // Apply initial liquid type selection style after visual tree is ready
        ApplyInitialLiquidTypeStyle(0);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnSaved -= OnSaved;
        _viewModel.OnCancelled -= OnCancelled;
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ManageLiquidViewModel.SelectedQuickQuantity))
            UpdateButtonStyles(_viewModel.SelectedQuickQuantity);
    }

    private void ApplyInitialLiquidTypeStyle(int attempt)
    {
        if (_viewModel.SelectedLiquidType == null || attempt > 10) return;

        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(150), () =>
        {
            var found = false;
            var selectedItem = _viewModel.SelectedLiquidType;
            foreach (var child in LiquidTypesLayout.GetVisualTreeDescendants())
            {
                if (child is Border border && border.BindingContext is DTOs.Nutrition.LiquidType.LiquidTypeDTO dto)
                {
                    var isSelected = dto == selectedItem;
                    UpdateLiquidTypeItemStyle(border, isSelected);
                    if (isSelected) found = true;
                }
            }

            if (!found)
                ApplyInitialLiquidTypeStyle(attempt + 1);
        });
    }

    private void OnLiquidTypeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        UpdateAllLiquidTypeStyles();
    }

    private void UpdateAllLiquidTypeStyles()
    {
        var selectedItem = _viewModel.SelectedLiquidType;
        foreach (var child in LiquidTypesLayout.GetVisualTreeDescendants())
        {
            if (child is Border border && border.BindingContext is DTOs.Nutrition.LiquidType.LiquidTypeDTO dto)
                UpdateLiquidTypeItemStyle(border, dto == selectedItem);
        }
    }

    private static void UpdateLiquidTypeItemStyle(Border border, bool isSelected)
    {
        var stack = border.Content as HorizontalStackLayout;
        if (stack == null) return;

        string? liquidName = null;

        foreach (var child in stack.Children)
        {
            if (child is Label label)
            {
                liquidName ??= label.Text;
                label.TextColor = isSelected ? Colors.White : Color.FromArgb("#1A1918");
                label.FontFamily = isSelected ? "OutfitSemiBold" : "OutfitRegular";
            }
        }

        // Update icon source based on liquid type name
        foreach (var child in stack.Children)
        {
            if (child is Image image && liquidName != null)
            {
                image.Source = GetIconForLiquid(liquidName, isSelected);
                break;
            }
        }
    }

    private static string GetIconForLiquid(string name, bool isWhite)
    {
        var lower = name.ToLowerInvariant();
        if (lower.Contains("café") || lower.Contains("cafe") || lower.Contains("coffee"))
            return isWhite ? "coffe_white.svg" : "coffe.svg";
        if (lower.Contains("chá") || lower.Contains("cha") || lower.Contains("tea"))
            return isWhite ? "tea_white.svg" : "tea.svg";
        return isWhite ? "water_white.svg" : "water.svg";
    }

    private void UpdateButtonStyles(string? selectedValue)
    {
        foreach (var (value, (border, label)) in _quickButtons)
        {
            if (value == selectedValue)
            {
                border.BackgroundColor = Color.FromArgb("#C8F0D8");
                border.StrokeThickness = 0;
                border.Stroke = Colors.Transparent;
                label.TextColor = Color.FromArgb("#3D8A5A");
                label.FontFamily = "OutfitSemiBold";
            }
            else
            {
                border.BackgroundColor = Colors.White;
                border.StrokeThickness = 1;
                border.Stroke = Color.FromArgb("#E5E4E1");
                label.TextColor = Color.FromArgb("#1A1918");
                label.FontFamily = "OutfitRegular";
            }
        }
    }

    private async void OnSaved(object? sender, EventArgs e)
    {
        _nutritionViewModel.InvalidateDataCache();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}

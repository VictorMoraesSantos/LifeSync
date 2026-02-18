using LifeSyncApp.ViewModels.Financial;
using System.Windows.Input;

namespace LifeSyncApp.Views.Financial;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoriesViewModel _viewModel;

    public ICommand BackButtonCommand { get; }

    public CategoriesPage(CategoriesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        BackButtonCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}

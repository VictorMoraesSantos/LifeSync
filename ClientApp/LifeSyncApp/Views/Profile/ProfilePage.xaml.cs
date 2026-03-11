using LifeSyncApp.ViewModels.Profile;

namespace LifeSyncApp.Views.Profile;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnBackTapped(object? sender, EventArgs e)
    {
        // The back arrow navigates to the first tab (Financeiro)
        // In the MainPage tab system, we use MessagingCenter to communicate
        MessagingCenter.Send<object, int>(this, "SelectTab", 0);
    }
}

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
        MessagingCenter.Send<object>(this, "GoBackTab");
    }
}

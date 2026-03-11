using CommunityToolkit.Mvvm.Messaging;
using LifeSyncApp.Messages;
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
        WeakReferenceMessenger.Default.Send(new GoBackTabMessage());
    }
}

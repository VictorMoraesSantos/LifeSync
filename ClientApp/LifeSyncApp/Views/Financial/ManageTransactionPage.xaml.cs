using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Views.Financial;

public partial class ManageTransactionPage : ContentPage
{
    public ManageTransactionPage(ManageTransactionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

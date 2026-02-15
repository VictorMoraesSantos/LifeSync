using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Views.Financial
{
    public partial class FilterTransactionModal : ContentPage
    {
        private readonly FilterTransactionViewModel _viewModel;

        public FilterTransactionModal(FilterTransactionViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            _viewModel.OnFiltersApplied += async (s, filter) =>
            {
                await Shell.Current.GoToAsync("..", new Dictionary<string, object>
                {
                    { "Filter", filter }
                });
            };

            _viewModel.OnCancelled += async (s, e) => await Shell.Current.GoToAsync("..");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }
    }
}

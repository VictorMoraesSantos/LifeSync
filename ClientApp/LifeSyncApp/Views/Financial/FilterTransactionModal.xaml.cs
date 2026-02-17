using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Views.Financial
{
    [QueryProperty(nameof(ExistingFilter), "ExistingFilter")]
    public partial class FilterTransactionModal : ContentPage
    {
        private readonly FilterTransactionViewModel _viewModel;

        public TransactionFilterDTO? ExistingFilter { get; set; }

        public FilterTransactionModal(FilterTransactionViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.OnFiltersApplied += OnFiltersApplied;
            _viewModel.OnCancelled += OnCancelled;

            await _viewModel.InitializeAsync(ExistingFilter);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewModel.OnFiltersApplied -= OnFiltersApplied;
            _viewModel.OnCancelled -= OnCancelled;
        }

        private async void OnFiltersApplied(object? sender, TransactionFilterDTO filter)
        {
            await Shell.Current.GoToAsync("..", new Dictionary<string, object>
            {
                { "Filter", filter }
            });
        }

        private async void OnCancelled(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

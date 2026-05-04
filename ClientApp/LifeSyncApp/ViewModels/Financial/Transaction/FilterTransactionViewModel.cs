using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Helpers;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;

namespace LifeSyncApp.ViewModels.Financial.Transaction
{
    public partial class FilterTransactionViewModel : BaseViewModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserSession _userSession;

        [ObservableProperty]
        private string _selectedType = "";

        [ObservableProperty]
        private string _selectedPaymentMethod = "";

        [ObservableProperty]
        private string _selectedDateFilter = "";

        public ObservableCollection<SelectableCategoryItem> SelectableCategories { get; } = new();

        public event EventHandler<TransactionFilterDTO>? OnFiltersApplied;
        public event EventHandler? OnCancelled;

        public FilterTransactionViewModel(ICategoryService categoryService, IUserSession userSession)
        {
            _categoryService = categoryService;
            _userSession = userSession;
            Title = "Filtrar Transações";
        }

        public async Task InitializeAsync(TransactionFilterDTO? existingFilter = null)
        {
            var categories = await _categoryService.GetCategoriesByUserIdAsync(_userSession.UserId);

            var allItems = new List<SelectableCategoryItem>
            {
                new SelectableCategoryItem { Id = -1, Name = "Todas as categorias", IsSelected = true }
            };
            allItems.AddRange(categories.Select(c => new SelectableCategoryItem { Id = c.Id, Name = c.Name }));
            SelectableCategories.ReplaceAll(allItems);

            if (existingFilter != null)
                ApplyExistingFilter(existingFilter);
        }

        private void ApplyExistingFilter(TransactionFilterDTO filter)
        {
            SelectedType = filter.TransactionType switch
            {
                TransactionType.Income => "Income",
                TransactionType.Expense => "Expense",
                _ => ""
            };

            SelectedPaymentMethod = filter.PaymentMethod switch
            {
                PaymentMethod.Cash => "Cash",
                PaymentMethod.CreditCard => "CreditCard",
                PaymentMethod.DebitCard => "DebitCard",
                PaymentMethod.BankTransfer => "BankTransfer",
                PaymentMethod.Pix => "Pix",
                PaymentMethod.Other => "Other",
                _ => ""
            };

            if (filter.CategoryId.HasValue)
            {
                foreach (var cat in SelectableCategories)
                    cat.IsSelected = cat.Id == filter.CategoryId.Value;
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            SelectedDateFilter = filter.TransactionDateFrom switch
            {
                var d when d == today && filter.TransactionDateTo == today => "Today",
                var d when d == today.AddDays(-(int)DateTime.Today.DayOfWeek) => "ThisWeek",
                var d when d == new DateOnly(today.Year, today.Month, 1) => "ThisMonth",
                _ => ""
            };
        }

        [RelayCommand]
        private void SelectType(string t)
        {
            SelectedType = t ?? "";
        }

        [RelayCommand]
        private void SelectPaymentMethod(string p)
        {
            SelectedPaymentMethod = p ?? "";
        }

        [RelayCommand]
        private void SelectDateFilter(string d)
        {
            SelectedDateFilter = d ?? "";
        }

        [RelayCommand]
        private void SelectCategory(SelectableCategoryItem? item)
        {
            if (item == null) return;

            foreach (var cat in SelectableCategories)
                cat.IsSelected = false;

            item.IsSelected = true;
        }

        [RelayCommand]
        private void Apply()
        {
            TransactionType? selectedType = SelectedType switch
            {
                "Income" => TransactionType.Income,
                "Expense" => TransactionType.Expense,
                _ => null
            };

            PaymentMethod? selectedPaymentMethod = SelectedPaymentMethod switch
            {
                "Cash" => PaymentMethod.Cash,
                "CreditCard" => PaymentMethod.CreditCard,
                "DebitCard" => PaymentMethod.DebitCard,
                "BankTransfer" => PaymentMethod.BankTransfer,
                "Pix" => PaymentMethod.Pix,
                "Other" => PaymentMethod.Other,
                _ => null
            };

            int? categoryId = null;
            var selectedCat = SelectableCategories.FirstOrDefault(c => c.IsSelected && c.Id != -1);
            if (selectedCat != null)
                categoryId = selectedCat.Id;

            DateOnly? dateFrom = null;
            DateOnly? dateTo = null;
            var today = DateOnly.FromDateTime(DateTime.Today);

            switch (SelectedDateFilter)
            {
                case "Today":
                    dateFrom = today;
                    dateTo = today;
                    break;
                case "ThisWeek":
                    var startOfWeek = today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    dateFrom = startOfWeek;
                    dateTo = today;
                    break;
                case "ThisMonth":
                    dateFrom = new DateOnly(today.Year, today.Month, 1);
                    dateTo = today;
                    break;
            }

            var filter = new TransactionFilterDTO(
                UserId: _userSession.UserId,
                TransactionType: selectedType,
                CategoryId: categoryId,
                PaymentMethod: selectedPaymentMethod,
                TransactionDateFrom: dateFrom,
                TransactionDateTo: dateTo
            );

            OnFiltersApplied?.Invoke(this, filter);
        }

        [RelayCommand]
        private void Clear()
        {
            SelectedType = "";
            SelectedPaymentMethod = "";
            SelectedDateFilter = "";
            foreach (var cat in SelectableCategories)
                cat.IsSelected = cat.Id == -1;
        }

        [RelayCommand]
        private void Cancel()
        {
            OnCancelled?.Invoke(this, EventArgs.Empty);
        }
    }
}

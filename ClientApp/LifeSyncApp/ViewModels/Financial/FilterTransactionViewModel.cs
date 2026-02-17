using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Financial
{
    public class FilterTransactionViewModel : ViewModels.BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação

        private string _selectedType = "";
        private string _selectedPaymentMethod = "";
        private string _selectedDateFilter = "";

        public string SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }

        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set => SetProperty(ref _selectedPaymentMethod, value);
        }

        public string SelectedDateFilter
        {
            get => _selectedDateFilter;
            set => SetProperty(ref _selectedDateFilter, value);
        }

        public ObservableCollection<SelectableCategoryItem> SelectableCategories { get; } = new();

        public ICommand SelectTypeCommand { get; }
        public ICommand SelectPaymentMethodCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand SelectDateFilterCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler<TransactionFilterDTO>? OnFiltersApplied;
        public event EventHandler? OnCancelled;

        public FilterTransactionViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
            Title = "Filtrar Transações";

            SelectTypeCommand = new Command<string>(t => SelectedType = t ?? "");
            SelectPaymentMethodCommand = new Command<string>(p => SelectedPaymentMethod = p ?? "");
            SelectDateFilterCommand = new Command<string>(d => SelectedDateFilter = d ?? "");
            SelectCategoryCommand = new Command<SelectableCategoryItem>(OnSelectCategory);

            ApplyCommand = new Command(OnApply);
            ClearCommand = new Command(OnClear);
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
        }

        public async Task InitializeAsync(TransactionFilterDTO? existingFilter = null)
        {
            var categories = await _categoryService.GetCategoriesByUserIdAsync(_userId);
            SelectableCategories.Clear();

            SelectableCategories.Add(new SelectableCategoryItem { Id = -1, Name = "Todas as categorias", IsSelected = true });

            foreach (var category in categories)
            {
                SelectableCategories.Add(new SelectableCategoryItem { Id = category.Id, Name = category.Name });
            }

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

        private void OnSelectCategory(SelectableCategoryItem? item)
        {
            if (item == null) return;
            foreach (var cat in SelectableCategories)
                cat.IsSelected = false;
            item.IsSelected = true;
        }

        private void OnApply()
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
                UserId: _userId,
                TransactionType: selectedType,
                CategoryId: categoryId,
                PaymentMethod: selectedPaymentMethod,
                TransactionDateFrom: dateFrom,
                TransactionDateTo: dateTo
            );

            OnFiltersApplied?.Invoke(this, filter);
        }

        private void OnClear()
        {
            SelectedType = "";
            SelectedPaymentMethod = "";
            SelectedDateFilter = "";
            foreach (var cat in SelectableCategories)
                cat.IsSelected = cat.Id == -1;
        }
    }
}

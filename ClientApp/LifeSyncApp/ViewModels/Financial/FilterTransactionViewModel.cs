using LifeSyncApp.DTOs.Financial.Category;
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

        private int _selectedTypeIndex = -1;
        private CategoryDTO? _selectedCategory;
        private int _selectedPaymentMethodIndex = -1;
        private DateTime _dateFrom = DateTime.Now.AddMonths(-1);
        private DateTime _dateTo = DateTime.Now;

        public int SelectedTypeIndex
        {
            get => _selectedTypeIndex;
            set => SetProperty(ref _selectedTypeIndex, value);
        }

        public CategoryDTO? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public int SelectedPaymentMethodIndex
        {
            get => _selectedPaymentMethodIndex;
            set => SetProperty(ref _selectedPaymentMethodIndex, value);
        }

        public DateTime DateFrom
        {
            get => _dateFrom;
            set => SetProperty(ref _dateFrom, value);
        }

        public DateTime DateTo
        {
            get => _dateTo;
            set => SetProperty(ref _dateTo, value);
        }

        public ObservableCollection<CategoryDTO> Categories { get; } = new();
        public ObservableCollection<string> TransactionTypes { get; } = new() { "Receita", "Despesa" };
        public ObservableCollection<string> PaymentMethods { get; } = new();

        public ICommand ApplyCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler<TransactionFilterDTO>? OnFiltersApplied;
        public event EventHandler? OnCancelled;

        public FilterTransactionViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
            Title = "Filtrar Transações";

            ApplyCommand = new Command(OnApply);
            ClearCommand = new Command(OnClear);
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));

            LoadPaymentMethods();
        }

        public async Task InitializeAsync()
        {
            var categories = await _categoryService.GetCategoriesByUserIdAsync(_userId);
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        private void LoadPaymentMethods()
        {
            foreach (PaymentMethod method in Enum.GetValues(typeof(PaymentMethod)))
            {
                PaymentMethods.Add(method.ToDisplayString());
            }
        }

        private void OnApply()
        {
            TransactionType? selectedType = null;
            if (SelectedTypeIndex >= 0 && SelectedTypeIndex < TransactionTypes.Count)
            {
                selectedType = SelectedTypeIndex == 0 ? TransactionType.Income : TransactionType.Expense;
            }

            PaymentMethod? selectedPaymentMethod = null;
            if (SelectedPaymentMethodIndex >= 0 && SelectedPaymentMethodIndex < PaymentMethods.Count)
            {
                var paymentMethodValues = Enum.GetValues(typeof(PaymentMethod)).Cast<PaymentMethod>().ToList();
                selectedPaymentMethod = paymentMethodValues[SelectedPaymentMethodIndex];
            }

            var filter = new TransactionFilterDTO(
                UserId: _userId,
                TransactionType: selectedType,
                CategoryId: SelectedCategory?.Id,
                PaymentMethod: selectedPaymentMethod,
                TransactionDateFrom: DateOnly.FromDateTime(DateFrom),
                TransactionDateTo: DateOnly.FromDateTime(DateTo)
            );

            OnFiltersApplied?.Invoke(this, filter);
        }

        private void OnClear()
        {
            SelectedTypeIndex = -1;
            SelectedCategory = null;
            SelectedPaymentMethodIndex = -1;
            DateFrom = DateTime.Now.AddMonths(-1);
            DateTo = DateTime.Now;
        }
    }
}

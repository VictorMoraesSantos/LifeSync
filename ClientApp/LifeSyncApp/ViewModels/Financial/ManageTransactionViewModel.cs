using System.Collections.ObjectModel;
using System.Windows.Input;
using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;

namespace LifeSyncApp.ViewModels.Financial
{
    public class ManageTransactionViewModel : ViewModels.BaseViewModel
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação

        private TransactionDTO? _transaction;
        private string _description = string.Empty;
        private decimal _amount;
        private DateTime _transactionDate = DateTime.Now;
        private TransactionType _transactionType = TransactionType.Expense;
        private PaymentMethod _paymentMethod = PaymentMethod.Cash;
        private PaymentMethodItem? _selectedPaymentMethodItem;
        private CategoryDTO? _selectedCategory;
        private bool _isEditing;

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public DateTime TransactionDate
        {
            get => _transactionDate;
            set => SetProperty(ref _transactionDate, value);
        }

        public TransactionType TransactionType
        {
            get => _transactionType;
            set => SetProperty(ref _transactionType, value);
        }

        public PaymentMethod PaymentMethod
        {
            get => _paymentMethod;
            set => SetProperty(ref _paymentMethod, value);
        }

        public PaymentMethodItem? SelectedPaymentMethodItem
        {
            get => _selectedPaymentMethodItem;
            set
            {
                if (SetProperty(ref _selectedPaymentMethodItem, value) && value != null)
                {
                    PaymentMethod = value.Value;
                }
            }
        }

        public CategoryDTO? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            private set => SetProperty(ref _isEditing, value);
        }

        public ObservableCollection<CategoryDTO> Categories { get; } = new();
        public ObservableCollection<TransactionType> TransactionTypes { get; } = new();
        public ObservableCollection<PaymentMethodItem> PaymentMethods { get; } = new();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SetTransactionTypeCommand { get; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageTransactionViewModel(TransactionService transactionService, CategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            Title = "Nova Transação";

            SaveCommand = new Command(async () => await SaveAsync(), CanSave);
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
            SetTransactionTypeCommand = new Command<string>(SetTransactionType);

            LoadEnums();
        }

        private void LoadEnums()
        {
            foreach (TransactionType type in Enum.GetValues(typeof(TransactionType)))
            {
                TransactionTypes.Add(type);
            }

            foreach (PaymentMethod method in Enum.GetValues(typeof(PaymentMethod)))
            {
                PaymentMethods.Add(new PaymentMethodItem
                {
                    Value = method,
                    DisplayName = method.ToDisplayString()
                });
            }
        }

        private void SetTransactionType(string typeString)
        {
            if (Enum.TryParse<TransactionType>(typeString, out var type))
            {
                TransactionType = type;
            }
        }

        public async Task InitializeAsync(TransactionDTO? transaction = null)
        {
            _transaction = transaction;
            IsEditing = transaction != null;
            Title = IsEditing ? "Editar Transação" : "Nova Transação";

            // Carregar categorias
            System.Diagnostics.Debug.WriteLine("Loading categories for transaction...");
            var categories = await _categoryService.GetCategoriesByUserIdAsync(_userId);
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
            System.Diagnostics.Debug.WriteLine($"Loaded {categories.Count} categories for transaction");

            if (IsEditing && _transaction != null)
            {
                Description = _transaction.Description;
                Amount = _transaction.Amount.ToDecimal();
                TransactionDate = _transaction.TransactionDate;
                TransactionType = _transaction.TransactionType;
                PaymentMethod = _transaction.PaymentMethod;
                SelectedPaymentMethodItem = PaymentMethods.FirstOrDefault(p => p.Value == _transaction.PaymentMethod);
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == _transaction.Category?.Id);
            }
            else
            {
                // Set default payment method item
                SelectedPaymentMethodItem = PaymentMethods.FirstOrDefault(p => p.Value == PaymentMethod.Cash);
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Description) && Amount > 0;
        }

        private async Task SaveAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                if (IsEditing && _transaction != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Updating transaction {_transaction.Id}...");
                    var dto = new UpdateTransactionDTO
                    {
                        Description = Description,
                        Amount = Money.FromDecimal(Amount),
                        TransactionDate = TransactionDate,
                        TransactionType = TransactionType,
                        PaymentMethod = PaymentMethod,
                        CategoryId = SelectedCategory?.Id
                    };

                    var success = await _transactionService.UpdateTransactionAsync(_transaction.Id, dto);
                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine("Transaction updated successfully");
                        OnSaved?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to update transaction");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Creating new transaction...");
                    var dto = new CreateTransactionDTO
                    {
                        UserId = _userId,
                        Description = Description,
                        Amount = Money.FromDecimal(Amount),
                        TransactionDate = TransactionDate,
                        TransactionType = TransactionType,
                        PaymentMethod = PaymentMethod,
                        CategoryId = SelectedCategory?.Id
                    };

                    var id = await _transactionService.CreateTransactionAsync(dto);
                    if (id.HasValue)
                    {
                        System.Diagnostics.Debug.WriteLine($"Transaction created with ID: {id.Value}");
                        OnSaved?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to create transaction");
                        await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível criar a transação. Verifique sua conexão e tente novamente.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving transaction: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    public class PaymentMethodItem
    {
        public PaymentMethod Value { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}

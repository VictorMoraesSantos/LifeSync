using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Models.Financial.Category;
using LifeSyncApp.Models.Financial.Transaction;
using LifeSyncApp.Helpers;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;

namespace LifeSyncApp.ViewModels.Financial.Transaction
{
    public partial class ManageTransactionViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;
        private readonly IUserSession _userSession;

        private TransactionDTO? _transaction;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private decimal _amount;

        [ObservableProperty]
        private DateTime _transactionDate = DateTime.Now;

        [ObservableProperty]
        private TransactionType _transactionType = TransactionType.Expense;

        [ObservableProperty]
        private PaymentMethod _paymentMethod = PaymentMethod.Cash;

        [ObservableProperty]
        private CategoryDTO? _selectedCategory;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        private bool _isEditing;

        public string SaveButtonText => IsEditing ? "Editar" : "Criar";

        [ObservableProperty]
        private bool _isRecurring;

        [ObservableProperty]
        private RecurrenceFrequency _selectedFrequency = RecurrenceFrequency.Monthly;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RecurrenceEndDateValue))]
        private DateTime? _recurrenceEndDate;

        [ObservableProperty]
        private int? _maxOccurrences;

        public DateTime RecurrenceEndDateValue
        {
            get => RecurrenceEndDate ?? TransactionDate.AddMonths(3);
            set => RecurrenceEndDate = value;
        }

        public bool HasRecurrenceEndDate
        {
            get => RecurrenceEndDate.HasValue;
            set
            {
                if (value && !RecurrenceEndDate.HasValue)
                    RecurrenceEndDate = TransactionDate.AddMonths(3);
                else if (!value)
                    RecurrenceEndDate = null;
                OnPropertyChanged(nameof(HasRecurrenceEndDate));
                OnPropertyChanged(nameof(RecurrenceEndDateValue));
            }
        }

        public bool HasMaxOccurrences
        {
            get => MaxOccurrences.HasValue;
            set
            {
                if (value && !MaxOccurrences.HasValue)
                    MaxOccurrences = 12;
                else if (!value)
                    MaxOccurrences = null;
                OnPropertyChanged(nameof(HasMaxOccurrences));
            }
        }

        public string MaxOccurrencesText
        {
            get => MaxOccurrences?.ToString() ?? string.Empty;
            set
            {
                if (int.TryParse(value, out var parsed) && parsed > 0)
                    MaxOccurrences = parsed;
                else if (string.IsNullOrWhiteSpace(value))
                    MaxOccurrences = null;
            }
        }

        public ObservableCollection<CategoryDTO> Categories { get; } = new();
        public ObservableCollection<TransactionType> TransactionTypes { get; } = new();
        public ObservableCollection<SelectablePaymentMethodItem> PaymentMethods { get; } = new();
        public ObservableCollection<RecurrenceFrequency> Frequencies { get; } = new();

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageTransactionViewModel(ITransactionService transactionService, ICategoryService categoryService, IUserSession userSession)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _userSession = userSession;
            Title = "Nova Transação";

            LoadEnums();
        }

        private void LoadEnums()
        {
            foreach (TransactionType type in Enum.GetValues(typeof(TransactionType)))
                TransactionTypes.Add(type);

            foreach (RecurrenceFrequency frequency in Enum.GetValues(typeof(RecurrenceFrequency)))
                Frequencies.Add(frequency);

            foreach (PaymentMethod method in Enum.GetValues(typeof(PaymentMethod)))
            {
                PaymentMethods.Add(new SelectablePaymentMethodItem
                {
                    Value = method,
                    DisplayName = method.ToDisplayString(),
                    IconSource = "wallet.svg",
                    IsSelected = method == PaymentMethod.Cash
                });
            }
        }

        [RelayCommand]
        private void SetTransactionType(string typeString)
        {
            if (Enum.TryParse<TransactionType>(typeString, out var type))
                TransactionType = type;
        }

        [RelayCommand]
        private void SetFrequency(string frequencyString)
        {
            if (Enum.TryParse<RecurrenceFrequency>(frequencyString, out var frequency))
                SelectedFrequency = frequency;
        }

        [RelayCommand]
        private void TogglePaymentMethod(SelectablePaymentMethodItem? item)
        {
            if (item == null) return;

            foreach (var method in PaymentMethods)
                method.IsSelected = false;

            item.IsSelected = true;
            PaymentMethod = item.Value;
        }

        public async Task InitializeAsync(TransactionDTO? transaction = null)
        {
            _transaction = transaction;
            IsEditing = transaction != null;
            Title = IsEditing ? "Editar Transação" : "Nova Transação";

            var categories = await _categoryService.GetCategoriesByUserIdAsync(_userSession.UserId);
            Categories.ReplaceAll(categories);

            if (IsEditing && _transaction != null)
            {
                Description = _transaction.Description;
                Amount = _transaction.Amount.ToDecimal();
                TransactionDate = _transaction.TransactionDate;
                TransactionType = _transaction.TransactionType;
                PaymentMethod = _transaction.PaymentMethod;
                IsRecurring = _transaction.IsRecurring;

                foreach (var method in PaymentMethods)
                    method.IsSelected = method.Value == _transaction.PaymentMethod;

                SelectedCategory = Categories.FirstOrDefault(c => c.Id == _transaction.Category?.Id);
            }
            else
            {
                Description = string.Empty;
                Amount = 0;
                TransactionDate = DateTime.Now;
                TransactionType = TransactionType.Expense;
                PaymentMethod = PaymentMethod.Cash;
                SelectedCategory = null;
                IsRecurring = false;
                SelectedFrequency = RecurrenceFrequency.Monthly;
                RecurrenceEndDate = null;
                MaxOccurrences = null;

                foreach (var method in PaymentMethods)
                    method.IsSelected = false;

                var defaultMethod = PaymentMethods.FirstOrDefault(p => p.Value == PaymentMethod.Cash);
                if (defaultMethod != null)
                    defaultMethod.IsSelected = true;
            }
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(Description) && Amount > 0;

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy) return;

            if (!CanSave())
            {
                await Shell.Current.DisplayAlert("Atenção", "Preencha a descrição e informe um valor maior que zero.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                if (IsEditing && _transaction != null)
                {
                    var dto = new UpdateTransactionDTO(
                        _transaction.Id,
                        SelectedCategory?.Id,
                        PaymentMethod,
                        TransactionType,
                        Money.FromDecimal(Amount),
                        Description,
                        TransactionDate);

                    var (success, error) = await _transactionService.UpdateTransactionAsync(_transaction.Id, dto);
                    if (success)
                        OnSaved?.Invoke(this, EventArgs.Empty);
                    else
                        await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível atualizar a transação.", "OK");
                }
                else
                {
                    var dto = new CreateTransactionDTO(
                        _userSession.UserId,
                        SelectedCategory?.Id,
                        PaymentMethod,
                        TransactionType,
                        Money.FromDecimal(Amount),
                        Description,
                        TransactionDate,
                        IsRecurring,
                        IsRecurring ? SelectedFrequency : null,
                        IsRecurring ? RecurrenceEndDate : null,
                        IsRecurring ? MaxOccurrences : null);

                    var (id, error) = await _transactionService.CreateTransactionAsync(dto);
                    if (id.HasValue)
                        OnSaved?.Invoke(this, EventArgs.Empty);
                    else
                        await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível criar a transação.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            OnCancelled?.Invoke(this, EventArgs.Empty);
        }
    }
}

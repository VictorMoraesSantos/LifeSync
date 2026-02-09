using System.ComponentModel;
using System.Windows.Input;
using LifeSyncApp.DTOs.Financial;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Models.Financial.Enums;
using LifeSyncApp.Services.Financial;

namespace LifeSyncApp.ViewModels.Financial;

public class ManageTransactionViewModel : INotifyPropertyChanged, IQueryAttributable
{
    private readonly FinancialService _financialService;
    private bool _isBusy;
    private int? _transactionId;
    private decimal _amount;
    private string _description = string.Empty;
    private DateTime _transactionDate;
    private TransactionType _selectedType;
    private PaymentMethod _selectedPaymentMethod;
    private int _categoryId;
    private bool _isEditMode;

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }
    }

    public bool IsNotBusy => !IsBusy;

    public bool IsEditMode
    {
        get => _isEditMode;
        set
        {
            if (_isEditMode != value)
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(PageTitle));
            }
        }
    }

    public string PageTitle => IsEditMode ? "Editar Transação" : "Nova Transação";

    public decimal Amount
    {
        get => _amount;
        set
        {
            if (_amount != value)
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    public DateTime TransactionDate
    {
        get => _transactionDate;
        set
        {
            if (_transactionDate != value)
            {
                _transactionDate = value;
                OnPropertyChanged(nameof(TransactionDate));
            }
        }
    }

    public TransactionType SelectedType
    {
        get => _selectedType;
        set
        {
            if (_selectedType != value)
            {
                _selectedType = value;
                OnPropertyChanged(nameof(SelectedType));
                OnPropertyChanged(nameof(IsIncome));
                OnPropertyChanged(nameof(IsExpense));
            }
        }
    }

    public PaymentMethod SelectedPaymentMethod
    {
        get => _selectedPaymentMethod;
        set
        {
            if (_selectedPaymentMethod != value)
            {
                _selectedPaymentMethod = value;
                OnPropertyChanged(nameof(SelectedPaymentMethod));
            }
        }
    }

    public int CategoryId
    {
        get => _categoryId;
        set
        {
            if (_categoryId != value)
            {
                _categoryId = value;
                OnPropertyChanged(nameof(CategoryId));
            }
        }
    }

    public bool IsIncome => SelectedType == TransactionType.Income;
    public bool IsExpense => SelectedType == TransactionType.Expense;

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand SelectTypeCommand { get; }

    public ManageTransactionViewModel(FinancialService financialService)
    {
        _financialService = financialService;
        _transactionDate = DateTime.Now;
        _selectedType = TransactionType.Expense;
        _selectedPaymentMethod = PaymentMethod.Cash;
        _categoryId = 1; // Default category

        SaveCommand = new Command(async () => await SaveAsync(), () => CanSave());
        CancelCommand = new Command(async () => await CancelAsync());
        SelectTypeCommand = new Command<TransactionType>(type => SelectedType = type);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var idObj) && int.TryParse(idObj?.ToString(), out var id))
        {
            _transactionId = id;
            IsEditMode = true;
            Task.Run(async () => await LoadTransactionAsync(id));
        }
        else
        {
            IsEditMode = false;
        }
    }

    private async Task LoadTransactionAsync(int id)
    {
        try
        {
            IsBusy = true;
            var dto = await _financialService.GetTransactionByIdAsync(id);
            
            if (dto != null)
            {
                Amount = dto.Amount?.Amount ?? 0;
                Description = dto.Description;
                TransactionDate = dto.TransactionDate;
                SelectedType = dto.TransactionType;
                SelectedPaymentMethod = dto.PaymentMethod;
                CategoryId = dto.Category?.Id ?? 1;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar transação: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanSave()
    {
        return !IsBusy && Amount > 0 && !string.IsNullOrWhiteSpace(Description);
    }

    private async Task SaveAsync()
    {
        if (!CanSave()) return;

        try
        {
            IsBusy = true;

            if (IsEditMode && _transactionId.HasValue)
            {
                var updateDto = new UpdateTransactionDTO
                {
                    CategoryId = CategoryId,
                    PaymentMethod = SelectedPaymentMethod,
                    TransactionType = SelectedType,
                    Amount = Amount,
                    Currency = "BRL",
                    Description = Description,
                    TransactionDate = TransactionDate
                };

                var success = await _financialService.UpdateTransactionAsync(_transactionId.Value, updateDto);
                
                if (success)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Transação atualizada!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", "Não foi possível atualizar a transação.", "OK");
                }
            }
            else
            {
                var createDto = new CreateTransactionDTO
                {
                    UserId = 1, // TODO: Get from auth
                    CategoryId = CategoryId,
                    PaymentMethod = SelectedPaymentMethod,
                    TransactionType = SelectedType,
                    Amount = Amount,
                    Currency = "BRL",
                    Description = Description,
                    TransactionDate = TransactionDate,
                    IsRecurring = false
                };

                var id = await _financialService.CreateTransactionAsync(createDto);
                
                if (id.HasValue)
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Transação criada!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", "Não foi possível criar a transação.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao salvar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

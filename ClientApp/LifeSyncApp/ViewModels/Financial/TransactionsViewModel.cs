using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using LifeSyncApp.DTOs.Financial;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Models.Financial.Enums;
using LifeSyncApp.Services.Financial;

namespace LifeSyncApp.ViewModels.Financial;

public class TransactionsViewModel : INotifyPropertyChanged
{
    private readonly FinancialService _financialService;
    private bool _isBusy;
    private bool _isRefreshing;
    private Transaction? _selectedTransaction;
    private TransactionFilterDTO _currentFilter;
    private string _searchText = string.Empty;

    public ObservableCollection<Transaction> Transactions { get; } = new();
    public ObservableCollection<Transaction> FilteredTransactions { get; } = new();

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

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            if (_isRefreshing != value)
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
    }

    public Transaction? SelectedTransaction
    {
        get => _selectedTransaction;
        set
        {
            if (_selectedTransaction != value)
            {
                _selectedTransaction = value;
                OnPropertyChanged(nameof(SelectedTransaction));
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilters();
            }
        }
    }

    public ICommand LoadTransactionsCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand CreateTransactionCommand { get; }
    public ICommand EditTransactionCommand { get; }
    public ICommand DeleteTransactionCommand { get; }
    public ICommand FilterByTypeCommand { get; }

    public TransactionsViewModel(FinancialService financialService)
    {
        _financialService = financialService;
        _currentFilter = new TransactionFilterDTO { UserId = 1 }; // TODO: Get from auth

        LoadTransactionsCommand = new Command(async () => await LoadTransactionsAsync());
        RefreshCommand = new Command(async () => await RefreshAsync());
        SearchCommand = new Command<string>(async (query) => await SearchAsync(query));
        CreateTransactionCommand = new Command(async () => await CreateTransactionAsync());
        EditTransactionCommand = new Command<Transaction>(async (t) => await EditTransactionAsync(t));
        DeleteTransactionCommand = new Command<Transaction>(async (t) => await DeleteTransactionAsync(t));
        FilterByTypeCommand = new Command<TransactionType>(async (type) => await FilterByTypeAsync(type));
    }

    public async Task LoadTransactionsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            Transactions.Clear();

            var result = await _financialService.SearchTransactionsAsync(_currentFilter);
            
            foreach (var dto in result.Items)
            {
                Transactions.Add(MapDtoToModel(dto));
            }

            ApplyFilters();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar transações: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadTransactionsAsync();
        IsRefreshing = false;
    }

    public async Task SearchAsync(string? query)
    {
        _currentFilter.UserId = 1; // TODO: Get from auth
        await LoadTransactionsAsync();
    }

    public async Task FilterByTypeAsync(TransactionType type)
    {
        _currentFilter.TransactionType = type;
        await LoadTransactionsAsync();
    }

    public async Task CreateTransactionAsync()
    {
        // Navigate to create page
        await Shell.Current.GoToAsync("create-transaction");
    }

    public async Task EditTransactionAsync(Transaction transaction)
    {
        if (transaction == null) return;
        
        await Shell.Current.GoToAsync($"edit-transaction?id={transaction.Id}");
    }

    public async Task DeleteTransactionAsync(Transaction transaction)
    {
        if (transaction == null) return;

        var confirm = await Shell.Current.DisplayAlert(
            "Confirmar Exclusão",
            $"Deseja realmente excluir a transação '{transaction.Description}'?",
            "Sim",
            "Não");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            var success = await _financialService.DeleteTransactionAsync(transaction.Id);
            
            if (success)
            {
                Transactions.Remove(transaction);
                ApplyFilters();
                await Shell.Current.DisplayAlert("Sucesso", "Transação excluída com sucesso!", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro", "Não foi possível excluir a transação.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao excluir: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyFilters()
    {
        FilteredTransactions.Clear();

        var filtered = Transactions.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(t => 
                t.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                t.Category?.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true);
        }

        foreach (var transaction in filtered)
        {
            FilteredTransactions.Add(transaction);
        }
    }

    private Transaction MapDtoToModel(TransactionDTO dto)
    {
        return new Transaction
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Category = dto.Category != null ? new Category
            {
                Id = dto.Category.Id,
                Name = dto.Category.Name,
                Description = dto.Category.Description,
                Color = dto.Category.Color,
                Icon = dto.Category.Icon,
                UserId = dto.Category.UserId,
                CreatedAt = dto.Category.CreatedAt,
                UpdatedAt = dto.Category.UpdatedAt
            } : null,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            PaymentMethod = dto.PaymentMethod,
            TransactionType = dto.TransactionType,
            Amount = new Money(dto.Amount?.Amount ?? 0, dto.Amount?.Currency ?? "BRL"),
            Description = dto.Description,
            TransactionDate = dto.TransactionDate,
            IsRecurring = dto.IsRecurring
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

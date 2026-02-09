using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using LifeSyncApp.DTOs.Financial;
using LifeSyncApp.Models.Financial.Enums;
using LifeSyncApp.Services.Financial;

namespace LifeSyncApp.ViewModels.Financial;

public class TransactionsViewModel : INotifyPropertyChanged
{
    private readonly FinancialService _financialService;
    private bool _isBusy;
    private bool _isRefreshing;
    private TransactionDTO? _selectedTransaction;
    private string _searchText = string.Empty;

    public ObservableCollection<TransactionDTO> Transactions { get; } = new();
    public ObservableCollection<TransactionDTO> FilteredTransactions { get; } = new();

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

    public TransactionDTO? SelectedTransaction
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

    public TransactionsViewModel(FinancialService financialService)
    {
        _financialService = financialService;

        LoadTransactionsCommand = new Command(async () => await LoadTransactionsAsync());
        RefreshCommand = new Command(async () => await RefreshAsync());
        SearchCommand = new Command<string>(async (query) => await SearchAsync(query));
        CreateTransactionCommand = new Command(async () => await CreateTransactionAsync());
        EditTransactionCommand = new Command<TransactionDTO>(async (t) => await EditTransactionAsync(t));
        DeleteTransactionCommand = new Command<TransactionDTO>(async (t) => await DeleteTransactionAsync(t));
    }

    public async Task LoadTransactionsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            Transactions.Clear();

            var transactions = await _financialService.GetAllTransactionsAsync();
            
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
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
        await LoadTransactionsAsync();
    }

    public async Task CreateTransactionAsync()
    {
        await Shell.Current.GoToAsync("create-transaction");
    }

    public async Task EditTransactionAsync(TransactionDTO transaction)
    {
        if (transaction == null) return;
        
        await Shell.Current.GoToAsync($"edit-transaction?id={transaction.Id}");
    }

    public async Task DeleteTransactionAsync(TransactionDTO transaction)
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

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

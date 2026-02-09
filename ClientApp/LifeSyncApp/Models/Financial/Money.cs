using System.ComponentModel;

namespace LifeSyncApp.Models.Financial;

public class Money : INotifyPropertyChanged
{
    private decimal _amount;
    private string _currency;

    public decimal Amount
    {
        get => _amount;
        set
        {
            if (_amount != value)
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
                OnPropertyChanged(nameof(FormattedAmount));
            }
        }
    }

    public string Currency
    {
        get => _currency;
        set
        {
            if (_currency != value)
            {
                _currency = value;
                OnPropertyChanged(nameof(Currency));
                OnPropertyChanged(nameof(FormattedAmount));
            }
        }
    }

    public string FormattedAmount
    {
        get
        {
            return Currency switch
            {
                "BRL" => $"R$ {Amount:N2}",
                "USD" => $"$ {Amount:N2}",
                "EUR" => $"€ {Amount:N2}",
                _ => $"{Amount:N2}"
            };
        }
    }

    public Money()
    {
        _amount = 0;
        _currency = "BRL";
    }

    public Money(decimal amount, string currency = "BRL")
    {
        _amount = amount;
        _currency = currency;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

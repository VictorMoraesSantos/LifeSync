using System.ComponentModel;
using LifeSyncApp.Models.Financial.Enums;

namespace LifeSyncApp.Models.Financial;

public class Transaction : INotifyPropertyChanged
{
    private int _id;
    private int _userId;
    private Category? _category;
    private DateTime _createdAt;
    private DateTime? _updatedAt;
    private PaymentMethod _paymentMethod;
    private TransactionType _transactionType;
    private Money _amount;
    private string _description = string.Empty;
    private DateTime _transactionDate;
    private bool _isRecurring;

    public int Id
    {
        get => _id;
        set
        {
            if (_id != value)
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
    }

    public int UserId
    {
        get => _userId;
        set
        {
            if (_userId != value)
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }
    }

    public Category? Category
    {
        get => _category;
        set
        {
            if (_category != value)
            {
                _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }
    }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set
        {
            if (_createdAt != value)
            {
                _createdAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }
    }

    public DateTime? UpdatedAt
    {
        get => _updatedAt;
        set
        {
            if (_updatedAt != value)
            {
                _updatedAt = value;
                OnPropertyChanged(nameof(UpdatedAt));
            }
        }
    }

    public PaymentMethod PaymentMethod
    {
        get => _paymentMethod;
        set
        {
            if (_paymentMethod != value)
            {
                _paymentMethod = value;
                OnPropertyChanged(nameof(PaymentMethod));
                OnPropertyChanged(nameof(PaymentMethodDisplay));
                OnPropertyChanged(nameof(PaymentMethodIcon));
            }
        }
    }

    public TransactionType TransactionType
    {
        get => _transactionType;
        set
        {
            if (_transactionType != value)
            {
                _transactionType = value;
                OnPropertyChanged(nameof(TransactionType));
                OnPropertyChanged(nameof(TransactionTypeDisplay));
                OnPropertyChanged(nameof(TransactionTypeColor));
                OnPropertyChanged(nameof(TransactionTypeIcon));
            }
        }
    }

    public Money Amount
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
                OnPropertyChanged(nameof(FormattedDate));
            }
        }
    }

    public bool IsRecurring
    {
        get => _isRecurring;
        set
        {
            if (_isRecurring != value)
            {
                _isRecurring = value;
                OnPropertyChanged(nameof(IsRecurring));
            }
        }
    }

    // Computed Properties
    public string PaymentMethodDisplay => PaymentMethod.ToDisplayString();
    public string PaymentMethodIcon => PaymentMethod.ToIcon();
    public string TransactionTypeDisplay => TransactionType.ToDisplayString();
    public Color TransactionTypeColor => TransactionType.ToColor();
    public string TransactionTypeIcon => TransactionType.ToIcon();
    public string FormattedAmount => Amount?.FormattedAmount ?? "R$ 0,00";
    public string FormattedDate => TransactionDate.ToString("dd/MM/yyyy");

    public Transaction()
    {
        _amount = new Money();
        _transactionDate = DateTime.Now;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

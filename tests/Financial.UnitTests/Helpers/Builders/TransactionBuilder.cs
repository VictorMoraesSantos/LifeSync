using Financial.Domain.Entities;
using Financial.Domain.Enums;
using FinancialControl.Domain.ValueObjects;

namespace Financial.UnitTests.Helpers.Builders;

public class TransactionBuilder
{
    private int _userId = 1;
    private int? _categoryId = 1;
    private PaymentMethod _paymentMethod = PaymentMethod.Pix;
    private TransactionType _transactionType = TransactionType.Expense;
    private Money _amount = Money.Create(1000, Currency.BRL);
    private string _description = "Test Transaction";
    private DateTime _transactionDate = DateTime.UtcNow;
    private bool _isRecurring = false;

    public TransactionBuilder WithUserId(int userId)
    {
        _userId = userId;
        return this;
    }

    public TransactionBuilder WithCategoryId(int? categoryId)
    {
        _categoryId = categoryId;
        return this;
    }

    public TransactionBuilder WithPaymentMethod(PaymentMethod paymentMethod)
    {
        _paymentMethod = paymentMethod;
        return this;
    }

    public TransactionBuilder WithTransactionType(TransactionType transactionType)
    {
        _transactionType = transactionType;
        return this;
    }

    public TransactionBuilder WithAmount(Money amount)
    {
        _amount = amount;
        return this;
    }

    public TransactionBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TransactionBuilder WithTransactionDate(DateTime transactionDate)
    {
        _transactionDate = transactionDate;
        return this;
    }

    public TransactionBuilder WithIsRecurring(bool isRecurring)
    {
        _isRecurring = isRecurring;
        return this;
    }

    public Transaction Build()
    {
        return new Transaction(
            _userId,
            _categoryId,
            _paymentMethod,
            _transactionType,
            _amount,
            _description,
            _transactionDate,
            _isRecurring);
    }

    public static TransactionBuilder Default() => new();
}

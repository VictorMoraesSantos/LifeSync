using System;
using System.Collections.Generic;
using LifeSyncApp.Client.Models.Financial;
using LifeSyncApp.Client.Models.Financial.Category;
using LifeSyncApp.Client.Models.Financial.Transaction;

namespace LifeSyncApp.Client.Components.Financial
{
    public class TransactionEditModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Amount { get; set; }
        public Currency Currency { get; set; } = Currency.BRL;
        public TransactionType TransactionType { get; set; } = TransactionType.Expense;
        public int? CategoryId { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public bool IsRecurring { get; set; }
    }

    public class CategoryEditModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}

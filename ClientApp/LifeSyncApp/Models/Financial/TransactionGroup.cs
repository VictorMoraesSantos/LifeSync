using LifeSyncApp.Models.Financial.Transaction;
using System.Collections.ObjectModel;

namespace LifeSyncApp.Models.Financial
{
    public class TransactionGroup : ObservableCollection<TransactionDTO>
    {
        public DateTime Date { get; }
        public string DateDisplay { get; }
        public string TransactionCountDisplay { get; }

        public TransactionGroup(DateTime date, IEnumerable<TransactionDTO> transactions) : base(transactions)
        {
            Date = date;
            DateDisplay = date.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"));
            var count = this.Count;
            TransactionCountDisplay = count == 1 ? "1 transação" : $"{count} transações";
        }
    }
}

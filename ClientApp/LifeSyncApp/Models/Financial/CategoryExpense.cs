namespace LifeSyncApp.Models.Financial
{
    public class CategoryExpense
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public double Percentage { get; set; }
        public double ProgressValue => Percentage / 100.0;
    }
}

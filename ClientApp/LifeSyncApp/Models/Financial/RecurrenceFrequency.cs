namespace LifeSyncApp.Models.Financial
{
    public enum RecurrenceFrequency
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }

    public static class RecurrenceFrequencyExtensions
    {
        public static string ToDisplayString(this RecurrenceFrequency frequency)
        {
            return frequency switch
            {
                RecurrenceFrequency.Daily => "Diário",
                RecurrenceFrequency.Weekly => "Semanal",
                RecurrenceFrequency.Monthly => "Mensal",
                RecurrenceFrequency.Yearly => "Anual",
                _ => frequency.ToString()
            };
        }
    }
}

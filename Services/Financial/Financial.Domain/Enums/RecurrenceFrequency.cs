namespace Financial.Domain.Enums
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
        public static string ToFriendlyString(this RecurrenceFrequency frequency)
        {
            return frequency switch
            {
                RecurrenceFrequency.Daily => "Daily",
                RecurrenceFrequency.Weekly => "Weekly",
                RecurrenceFrequency.Monthly => "Monthly",
                RecurrenceFrequency.Yearly => "Yearly",
                _ => "Unknown"
            };
        }
    }
}

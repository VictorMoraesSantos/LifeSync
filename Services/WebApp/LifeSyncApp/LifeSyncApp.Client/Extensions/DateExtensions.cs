using System;

namespace LifeSyncApp.Client.Extensions
{
    public static class DateExtensions
    {
        public static string ToFriendlyDateString(this DateOnly date)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var tomorrow = today.AddDays(1);
            var yesterday = today.AddDays(-1);

            if (date == today)
                return $"Hoje - {date:dd/MM/yyyy}";
            else if (date == tomorrow)
                return $"Amanhã - {date:dd/MM/yyyy}";
            else if (date == yesterday)
                return $"Ontem - {date:dd/MM/yyyy}";
            else if (date < today)
                return $"Atrasado - {date:dd/MM/yyyy}";
            else
                return date.ToString("dd/MM/yyyy - dddd");
        }
    }
}

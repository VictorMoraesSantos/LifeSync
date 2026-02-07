using LifeSyncApp.Models.TaskManager.Enums;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        private static readonly Color Low = Color.FromArgb("#2ECC71");
        private static readonly Color Medium = Color.FromArgb("#F39C12");
        private static readonly Color High = Color.FromArgb("#E74C3C");
        private static readonly Color Default = Color.FromArgb("#95A5A6");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Priority priority)
            {
                return priority switch
                {
                    Priority.Low => Low,
                    Priority.Medium => Medium,
                    Priority.High => High,
                    _ => Default
                };
            }
            return Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

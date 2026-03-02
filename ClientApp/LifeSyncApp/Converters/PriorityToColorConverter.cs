using LifeSyncApp.Models.TaskManager.Enums;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        private static readonly Color Low = Color.FromArgb("#3D8A5A");
        private static readonly Color Medium = Color.FromArgb("#D89575");
        private static readonly Color High = Color.FromArgb("#D08068");
        private static readonly Color Urgent = Color.FromArgb("#FF4444");
        private static readonly Color Default = Color.FromArgb("#9C9B99");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Priority priority)
            {
                return priority switch
                {
                    Priority.Low => Low,
                    Priority.Medium => Medium,
                    Priority.High => High,
                    Priority.Urgent => Urgent,
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

using LifeSyncApp.Models.TaskManager.Enums;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        internal static readonly Color Pending = Color.FromArgb("#95A5A6");
        internal static readonly Color InProgress = Color.FromArgb("#3498DB");
        internal static readonly Color Completed = Color.FromArgb("#2ECC71");
        internal static readonly Color Cancelled = Color.FromArgb("#E74C3C");
        internal static readonly Color Default = Color.FromArgb("#BDC3C7");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Status status)
            {
                return status switch
                {
                    Status.Pending => Pending,
                    Status.InProgress => InProgress,
                    Status.Completed => Completed,
                    Status.Cancelled => Cancelled,
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

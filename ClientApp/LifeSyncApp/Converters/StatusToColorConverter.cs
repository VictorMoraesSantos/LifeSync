using LifeSyncApp.Models.TaskManager.Enums;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        internal static readonly Color Pending = Color.FromArgb("#9C9B99");
        internal static readonly Color InProgress = Color.FromArgb("#D4A64A");
        internal static readonly Color Completed = Color.FromArgb("#3D8A5A");
        internal static readonly Color Cancelled = Color.FromArgb("#D08068");
        internal static readonly Color Default = Color.FromArgb("#9C9B99");

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

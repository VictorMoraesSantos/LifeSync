using LifeSyncApp.Models.TaskManager.Enums;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class StatusToLightColorConverter : IValueConverter
    {
        private static readonly Color PendingLight = StatusToColorConverter.Pending.WithAlpha(0.20f);
        private static readonly Color InProgressLight = StatusToColorConverter.InProgress.WithAlpha(0.20f);
        private static readonly Color CompletedLight = StatusToColorConverter.Completed.WithAlpha(0.20f);
        private static readonly Color CancelledLight = StatusToColorConverter.Cancelled.WithAlpha(0.20f);
        private static readonly Color DefaultLight = StatusToColorConverter.Default.WithAlpha(0.20f);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Status status)
            {
                return status switch
                {
                    Status.Pending => PendingLight,
                    Status.InProgress => InProgressLight,
                    Status.Completed => CompletedLight,
                    Status.Cancelled => CancelledLight,
                    _ => DefaultLight
                };
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using LifeSyncApp.Models.Financial;
using System.Globalization;

namespace LifeSyncApp.Converters.Financial
{
    public class FrequencyToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RecurrenceFrequency currentFrequency && parameter is string paramFrequency)
            {
                if (Enum.TryParse<RecurrenceFrequency>(paramFrequency, out var expectedFrequency))
                {
                    if (currentFrequency == expectedFrequency)
                        return Color.FromArgb("#EFF6FF");
                }
            }
            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

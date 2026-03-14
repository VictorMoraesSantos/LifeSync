using LifeSyncApp.Models.Financial;
using System.Globalization;

namespace LifeSyncApp.Converters.Financial
{
    public class FrequencyToBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RecurrenceFrequency currentFrequency && parameter is string paramFrequency)
            {
                if (Enum.TryParse<RecurrenceFrequency>(paramFrequency, out var expectedFrequency))
                {
                    if (currentFrequency == expectedFrequency)
                        return Color.FromArgb("#3B82F6");
                }
            }
            return Color.FromArgb("#E5E5E5");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

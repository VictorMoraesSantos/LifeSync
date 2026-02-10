using LifeSyncApp.Models.Financial;
using System.Globalization;

namespace LifeSyncApp.Converters.Financial
{
    public class TransactionTypeToBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TransactionType currentType && parameter is string paramType)
            {
                if (Enum.TryParse<TransactionType>(paramType, out var expectedType))
                {
                    if (currentType == expectedType)
                    {
                        return currentType == TransactionType.Income
                            ? Color.FromArgb("#00A63E") // Green
                            : Color.FromArgb("#E7000B"); // Red
                    }
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

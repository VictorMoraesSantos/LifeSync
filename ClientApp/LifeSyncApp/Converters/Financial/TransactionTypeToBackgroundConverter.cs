using LifeSyncApp.Models.Financial;
using System.Globalization;

namespace LifeSyncApp.Converters.Financial
{
    public class TransactionTypeToBackgroundConverter : IValueConverter
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
                            ? Color.FromArgb("#F0FDF4") // Light green
                            : Color.FromArgb("#FEF2F2"); // Light red
                    }
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

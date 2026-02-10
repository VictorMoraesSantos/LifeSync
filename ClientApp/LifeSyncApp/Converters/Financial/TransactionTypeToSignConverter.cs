using System.Globalization;
using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.Converters.Financial
{
    public class TransactionTypeToSignConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TransactionType type)
            {
                return type == TransactionType.Income ? "+" : "-";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

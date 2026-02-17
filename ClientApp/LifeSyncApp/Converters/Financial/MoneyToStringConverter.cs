using LifeSyncApp.Models.Financial;
using System.Globalization;

namespace LifeSyncApp.Converters.Financial
{
    public class MoneyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Money money)
            {
                return money.ToFormattedString();
            }

            if (value is decimal decimalValue)
            {
                return $"R$ {decimalValue:N2}";
            }

            return "R$ 0,00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

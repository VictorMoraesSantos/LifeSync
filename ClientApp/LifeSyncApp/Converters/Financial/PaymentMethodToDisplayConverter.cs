using System.Globalization;
using LifeSyncApp.Models.Financial;

namespace LifeSyncApp.Converters.Financial
{
    public class PaymentMethodToDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PaymentMethod method)
            {
                return method.ToDisplayString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

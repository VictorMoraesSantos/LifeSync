using System.Globalization;

namespace LifeSyncApp.Converters.Financial
{
    public class BoolToPaymentMethodBorderConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
                return isSelected ? Color.FromArgb("#171717") : Color.FromArgb("#E5E5E5");
            return Color.FromArgb("#E5E5E5");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

using System.Globalization;

namespace LifeSyncApp.Converters.Financial
{
    public class BoolToPaymentMethodTextColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
                return isSelected ? Colors.White : Color.FromArgb("#171717");
            return Color.FromArgb("#171717");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

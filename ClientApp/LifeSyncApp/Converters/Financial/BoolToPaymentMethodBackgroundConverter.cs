using System.Globalization;

namespace LifeSyncApp.Converters.Financial
{
    public class BoolToPaymentMethodBackgroundConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
                return isSelected ? Color.FromArgb("#171717") : Color.FromArgb("#F5F5F5");
            return Color.FromArgb("#F5F5F5");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

using System.Globalization;

namespace LifeSyncApp.Converters.Financial
{
    public class StringEqualToBackgroundConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var selected = value?.ToString() ?? "";
            var buttonValue = parameter?.ToString() ?? "";
            return selected == buttonValue ? Color.FromArgb("#171717") : Color.FromArgb("#F5F5F5");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

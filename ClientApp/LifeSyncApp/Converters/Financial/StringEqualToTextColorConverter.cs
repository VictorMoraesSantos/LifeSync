using System.Globalization;

namespace LifeSyncApp.Converters.Financial
{
    public class StringEqualToTextColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var selected = value?.ToString() ?? "";
            var buttonValue = parameter?.ToString() ?? "";
            return selected == buttonValue ? Colors.White : Color.FromArgb("#171717");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

using System.Globalization;

namespace LifeSyncApp.Converters.Nutrition
{
    public class LiquidNameToIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var name = value?.ToString()?.ToLowerInvariant() ?? "";

            if (name.Contains("café") || name.Contains("cafe") || name.Contains("coffee"))
                return "coffe.svg";

            if (name.Contains("chá") || name.Contains("cha") || name.Contains("tea"))
                return "tea.svg";

            return "water.svg";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

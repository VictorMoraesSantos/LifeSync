using System.Globalization;

namespace LifeSyncApp.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue || parameter is not string colors)
            return Colors.Gray;

        var colorPair = colors.Split(':');
        if (colorPair.Length != 2)
            return Colors.Gray;

        var trueColor = colorPair[0];
        var falseColor = colorPair[1];

        return Color.FromArgb(boolValue ? trueColor : falseColor);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

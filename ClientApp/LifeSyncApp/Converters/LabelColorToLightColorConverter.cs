using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class LabelColorToLightColorConverter : IValueConverter
    {
        private readonly LabelColorToConverter _baseConverter = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var baseColor = _baseConverter.Convert(value, targetType, parameter, culture) as Color;
            return baseColor?.WithAlpha(0.20f) ?? Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

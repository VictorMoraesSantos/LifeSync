using System.Collections;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class CollectionContainsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IEnumerable collection && parameter != null)
            {
                foreach (var item in collection)
                {
                    if (item?.Equals(parameter) == true)
                        return true;
                }
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

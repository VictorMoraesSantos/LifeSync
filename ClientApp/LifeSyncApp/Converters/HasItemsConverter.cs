using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class HasItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
                return count > 0;

            if (value is string str)
                return !string.IsNullOrWhiteSpace(str);

            if (value is System.Collections.ICollection collection)
                return collection.Count > 0;

            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
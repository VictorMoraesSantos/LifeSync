using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class TabBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selectedTab && parameter is string tabIndexStr && int.TryParse(tabIndexStr, out int tabIndex))
            {
                return selectedTab == tabIndex ? Color.FromArgb("#1A1918") : Colors.Transparent;
            }
            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

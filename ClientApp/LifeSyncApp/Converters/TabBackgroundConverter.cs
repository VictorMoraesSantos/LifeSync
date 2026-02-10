using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class TabBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selectedTab && parameter is string tabIndexStr && int.TryParse(tabIndexStr, out int tabIndex))
            {
                return selectedTab == tabIndex ? Color.FromArgb("#171717") : Colors.White;
            }
            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

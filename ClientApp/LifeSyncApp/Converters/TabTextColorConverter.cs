using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class TabTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selectedTab && parameter is string tabIndexStr && int.TryParse(tabIndexStr, out int tabIndex))
            {
                return selectedTab == tabIndex ? Colors.White : Color.FromArgb("#A8A7A5");
            }
            return Color.FromArgb("#A8A7A5");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

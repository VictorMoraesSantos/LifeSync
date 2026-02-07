using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class OverdueToColorConverter : IValueConverter
    {
        private static readonly Color OverdueColor = Colors.Red;
        private static readonly Color NormalColor = Color.FromArgb("#0078D4");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOverdue && isOverdue)
            {
                return OverdueColor;
            }
            return NormalColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

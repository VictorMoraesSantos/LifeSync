using LifeSyncApp.Models.TaskManager.Enums;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Priority priority)
            {
                return priority switch
                {
                    Priority.Low => Color.FromArgb("#2ECC71"),      // Verde
                    Priority.Medium => Color.FromArgb("#F39C12"),   // Laranja
                    Priority.High => Color.FromArgb("#E74C3C"),     // Vermelho
                    _ => Color.FromArgb("#95A5A6")                  // Cinza
                };
            }
            return Color.FromArgb("#95A5A6");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
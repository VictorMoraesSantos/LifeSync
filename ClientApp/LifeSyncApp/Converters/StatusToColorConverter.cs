using LifeSyncApp.Models.TaskManager.Enums;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Status status)
            {
                return status switch
                {
                    Status.Pending => Color.FromArgb("#95A5A6"),      // Cinza
                    Status.InProgress => Color.FromArgb("#3498DB"),   // Azul
                    Status.Completed => Color.FromArgb("#2ECC71"),    // Verde
                    Status.Cancelled => Color.FromArgb("#E74C3C"),    // Vermelho
                    _ => Color.FromArgb("#BDC3C7")                    // Cinza claro
                };
            }
            return Color.FromArgb("#BDC3C7");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
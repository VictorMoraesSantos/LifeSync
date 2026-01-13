using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.Converters
{

    public class StatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Status status)
            {
                return status switch
                {
                    Status.Pending => "○",
                    Status.InProgress => "◐",
                    Status.Completed => "✓",
                    Status.Cancelled => "✕",
                    _ => "○"
                };
            }
            return "○";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

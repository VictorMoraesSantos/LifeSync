using LifeSyncApp.Models.TaskManager.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeSyncApp.Converters
{
    public class LabelColorToConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is LabelColor color)
            {
                return color switch
                {
                    LabelColor.Purple => Color.FromArgb("#9C27B0"),
                    LabelColor.Pink => Color.FromArgb("#E91E63"),     
                    LabelColor.Blue => Color.FromArgb("#2196F3"),     
                    LabelColor.Brown => Color.FromArgb("#795548"),    
                    LabelColor.Red => Color.FromArgb("#F44336"),      
                    LabelColor.Gray => Color.FromArgb("#9E9E9E"),     
                    LabelColor.Green => Color.FromArgb("#4CAF50"),    
                    LabelColor.Orange => Color.FromArgb("#FF9800"),   
                    LabelColor.Yellow => Color.FromArgb("#FFC107"),   
                    _ => Color.FromArgb("#9E9E9E")                    
                };
            }
            return Color.FromArgb("#9E9E9E");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

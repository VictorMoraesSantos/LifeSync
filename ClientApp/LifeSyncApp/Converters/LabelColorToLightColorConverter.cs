using LifeSyncApp.Models.TaskManager.Enums;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class LabelColorToLightColorConverter : IValueConverter
    {
        private static readonly Color PurpleLight = Color.FromArgb("#9C27B0").WithAlpha(0.20f);
        private static readonly Color PinkLight = Color.FromArgb("#E91E63").WithAlpha(0.20f);
        private static readonly Color BlueLight = Color.FromArgb("#2196F3").WithAlpha(0.20f);
        private static readonly Color BrownLight = Color.FromArgb("#795548").WithAlpha(0.20f);
        private static readonly Color RedLight = Color.FromArgb("#F44336").WithAlpha(0.20f);
        private static readonly Color GrayLight = Color.FromArgb("#9E9E9E").WithAlpha(0.20f);
        private static readonly Color GreenLight = Color.FromArgb("#4CAF50").WithAlpha(0.20f);
        private static readonly Color OrangeLight = Color.FromArgb("#FF9800").WithAlpha(0.20f);
        private static readonly Color YellowLight = Color.FromArgb("#FFC107").WithAlpha(0.20f);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LabelColor color)
            {
                return color switch
                {
                    LabelColor.Purple => PurpleLight,
                    LabelColor.Pink => PinkLight,
                    LabelColor.Blue => BlueLight,
                    LabelColor.Brown => BrownLight,
                    LabelColor.Red => RedLight,
                    LabelColor.Gray => GrayLight,
                    LabelColor.Green => GreenLight,
                    LabelColor.Orange => OrangeLight,
                    LabelColor.Yellow => YellowLight,
                    _ => GrayLight
                };
            }
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

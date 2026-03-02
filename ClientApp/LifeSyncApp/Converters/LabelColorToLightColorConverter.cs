using LifeSyncApp.Models.TaskManager.Enums;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class LabelColorToLightColorConverter : IValueConverter
    {
        private static readonly Color PurpleLight = Color.FromArgb("#8B5CF6").WithAlpha(0.20f);
        private static readonly Color PinkLight = Color.FromArgb("#F472B6").WithAlpha(0.20f);
        private static readonly Color BlueLight = Color.FromArgb("#4A5FA8").WithAlpha(0.20f);
        private static readonly Color BrownLight = Color.FromArgb("#D89575").WithAlpha(0.20f);
        private static readonly Color RedLight = Color.FromArgb("#FF4444").WithAlpha(0.20f);
        private static readonly Color GrayLight = Color.FromArgb("#9C9B99").WithAlpha(0.20f);
        private static readonly Color GreenLight = Color.FromArgb("#3D8A5A").WithAlpha(0.20f);
        private static readonly Color OrangeLight = Color.FromArgb("#D89575").WithAlpha(0.20f);
        private static readonly Color YellowLight = Color.FromArgb("#D4A64A").WithAlpha(0.20f);

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

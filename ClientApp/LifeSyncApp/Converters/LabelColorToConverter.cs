using LifeSyncApp.Models.TaskManager.Enums;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class LabelColorToConverter : IValueConverter
    {
        private static readonly Color Purple = Color.FromArgb("#8B5CF6");
        private static readonly Color Pink = Color.FromArgb("#F472B6");
        private static readonly Color Blue = Color.FromArgb("#4A5FA8");
        private static readonly Color Brown = Color.FromArgb("#D89575");
        private static readonly Color Red = Color.FromArgb("#FF4444");
        private static readonly Color Gray = Color.FromArgb("#9C9B99");
        private static readonly Color Green = Color.FromArgb("#3D8A5A");
        private static readonly Color Orange = Color.FromArgb("#D89575");
        private static readonly Color Yellow = Color.FromArgb("#D4A64A");

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is LabelColor color)
            {
                return color switch
                {
                    LabelColor.Purple => Purple,
                    LabelColor.Pink => Pink,
                    LabelColor.Blue => Blue,
                    LabelColor.Brown => Brown,
                    LabelColor.Red => Red,
                    LabelColor.Gray => Gray,
                    LabelColor.Green => Green,
                    LabelColor.Orange => Orange,
                    LabelColor.Yellow => Yellow,
                    _ => Gray
                };
            }
            return Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

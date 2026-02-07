using LifeSyncApp.Models.TaskManager.Enums;
using System.Globalization;

namespace LifeSyncApp.Converters
{
    public class LabelColorToConverter : IValueConverter
    {
        private static readonly Color Purple = Color.FromArgb("#9C27B0");
        private static readonly Color Pink = Color.FromArgb("#E91E63");
        private static readonly Color Blue = Color.FromArgb("#2196F3");
        private static readonly Color Brown = Color.FromArgb("#795548");
        private static readonly Color Red = Color.FromArgb("#F44336");
        private static readonly Color Gray = Color.FromArgb("#9E9E9E");
        private static readonly Color Green = Color.FromArgb("#4CAF50");
        private static readonly Color Orange = Color.FromArgb("#FF9800");
        private static readonly Color Yellow = Color.FromArgb("#FFC107");

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

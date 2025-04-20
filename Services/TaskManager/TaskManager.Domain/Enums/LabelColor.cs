namespace TaskManager.Domain.Enums
{
    public enum LabelColor
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        Yellow = 3,
        Purple = 4,
        Orange = 5,
        Pink = 6,
        Brown = 7,
        Gray = 8,
    }

    public static class ColorExtensions
    {
        public static string ToHex(this LabelColor color)
        {
            return color switch
            {
                LabelColor.Red => "#FF0000",
                LabelColor.Green => "#00FF00",
                LabelColor.Blue => "#0000FF",
                LabelColor.Yellow => "#FFFF00",
                LabelColor.Purple => "#800080",
                LabelColor.Orange => "#FFA500",
                LabelColor.Pink => "#FFC0CB",
                LabelColor.Brown => "#A52A2A",
                LabelColor.Gray => "#808080",
                _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
            };
        }
    }
}

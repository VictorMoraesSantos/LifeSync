namespace LifeSyncApp.Controls
{
    public class CircularProgressView : GraphicsView
    {
        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create(nameof(Progress), typeof(double), typeof(CircularProgressView), 0.0, propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty ProgressColorProperty =
            BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(CircularProgressView), Colors.Green, propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty TrackColorProperty =
            BindableProperty.Create(nameof(TrackColor), typeof(Color), typeof(CircularProgressView), Color.FromArgb("#E5E4E1"), propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty StrokeWidthProperty =
            BindableProperty.Create(nameof(StrokeWidth), typeof(float), typeof(CircularProgressView), 6f, propertyChanged: OnPropertyChanged);

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public Color ProgressColor
        {
            get => (Color)GetValue(ProgressColorProperty);
            set => SetValue(ProgressColorProperty, value);
        }

        public Color TrackColor
        {
            get => (Color)GetValue(TrackColorProperty);
            set => SetValue(TrackColorProperty, value);
        }

        public float StrokeWidth
        {
            get => (float)GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }

        public CircularProgressView()
        {
            Drawable = new CircularProgressDrawable(this);
        }

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CircularProgressView view)
                view.Invalidate();
        }
    }

    public class CircularProgressDrawable : IDrawable
    {
        private readonly CircularProgressView _view;

        public CircularProgressDrawable(CircularProgressView view)
        {
            _view = view;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var size = Math.Min(dirtyRect.Width, dirtyRect.Height);
            var strokeWidth = _view.StrokeWidth;
            var radius = (size - strokeWidth) / 2;
            var centerX = dirtyRect.Width / 2;
            var centerY = dirtyRect.Height / 2;

            // Draw track (background circle)
            canvas.StrokeColor = _view.TrackColor;
            canvas.StrokeSize = strokeWidth;
            canvas.StrokeLineCap = LineCap.Round;
            canvas.DrawEllipse(centerX - radius, centerY - radius, radius * 2, radius * 2);

            // Draw progress arc
            var progress = Math.Clamp(_view.Progress, 0, 1);
            if (progress > 0)
            {
                canvas.StrokeColor = _view.ProgressColor;
                canvas.StrokeSize = strokeWidth;
                canvas.StrokeLineCap = LineCap.Round;

                var startAngle = 90f; // Start from top (12 o'clock)
                var sweepAngle = -(float)(progress * 360); // Clockwise

                var endAngle = startAngle + sweepAngle;
                canvas.DrawArc(
                    centerX - radius, centerY - radius,
                    radius * 2, radius * 2,
                    startAngle, endAngle,
                    clockwise: true,
                    closed: false);
            }
        }
    }
}

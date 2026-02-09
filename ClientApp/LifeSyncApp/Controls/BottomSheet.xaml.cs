using System.Windows.Input;

namespace LifeSyncApp.Controls;

public partial class BottomSheet : ContentView
{
    private const uint AnimationDuration = 300;
    private double _screenHeight;

    public static readonly BindableProperty IsOpenProperty =
        BindableProperty.Create(nameof(IsOpen), typeof(bool), typeof(BottomSheet), false, 
            propertyChanged: OnIsOpenChanged);

    public static readonly BindableProperty ContentProperty =
        BindableProperty.Create(nameof(Content), typeof(View), typeof(BottomSheet));

    public static readonly BindableProperty CloseCommandProperty =
        BindableProperty.Create(nameof(CloseCommand), typeof(ICommand), typeof(BottomSheet));

    public static readonly BindableProperty MaxHeightProperty =
        BindableProperty.Create(nameof(MaxHeight), typeof(double), typeof(BottomSheet), 600.0);

    public static readonly BindableProperty TranslationYProperty =
        BindableProperty.Create(nameof(TranslationY), typeof(double), typeof(BottomSheet), 1000.0);

    public static readonly BindableProperty OverlayOpacityProperty =
        BindableProperty.Create(nameof(OverlayOpacity), typeof(double), typeof(BottomSheet), 0.0);

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public View Content
    {
        get => (View)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public ICommand CloseCommand
    {
        get => (ICommand)GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    public double MaxHeight
    {
        get => (double)GetValue(MaxHeightProperty);
        set => SetValue(MaxHeightProperty, value);
    }

    public double TranslationY
    {
        get => (double)GetValue(TranslationYProperty);
        set => SetValue(TranslationYProperty, value);
    }

    public double OverlayOpacity
    {
        get => (double)GetValue(OverlayOpacityProperty);
        set => SetValue(OverlayOpacityProperty, value);
    }

    public BottomSheet()
    {
        InitializeComponent();
        _screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
    }

    private static async void OnIsOpenChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BottomSheet bottomSheet)
        {
            var isOpen = (bool)newValue;
            await bottomSheet.AnimateSheet(isOpen);
        }
    }

    private async Task AnimateSheet(bool open)
    {
        if (open)
        {
            // Open animation
            var translateTask = this.TranslateTo(0, 0, AnimationDuration, Easing.CubicOut);
            var opacityTask = Task.Run(async () =>
            {
                await Task.Delay(50);
                await MainThread.InvokeOnMainThreadAsync(() => OverlayOpacity = 1);
            });
            
            TranslationY = 0;
            await Task.WhenAll(translateTask, opacityTask);
        }
        else
        {
            // Close animation
            var translateTask = this.TranslateTo(0, _screenHeight, AnimationDuration, Easing.CubicIn);
            var opacityTask = Task.Run(async () =>
            {
                await MainThread.InvokeOnMainThreadAsync(() => OverlayOpacity = 0);
            });
            
            await Task.WhenAll(translateTask, opacityTask);
            TranslationY = _screenHeight;
        }
    }
}

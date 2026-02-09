using System.Windows.Input;

namespace LifeSyncApp.Controls;

public partial class BottomSheet : ContentView
{
    public static readonly BindableProperty IsOpenProperty =
        BindableProperty.Create(
            nameof(IsOpen),
            typeof(bool),
            typeof(BottomSheet),
            false,
            propertyChanged: OnIsOpenChanged);

    public static readonly BindableProperty ContentProperty =
        BindableProperty.Create(
            nameof(Content),
            typeof(View),
            typeof(BottomSheet),
            null);

    public static readonly BindableProperty MaxHeightProperty =
        BindableProperty.Create(
            nameof(MaxHeight),
            typeof(double),
            typeof(BottomSheet),
            600.0);

    public static readonly BindableProperty CloseCommandProperty =
        BindableProperty.Create(
            nameof(CloseCommand),
            typeof(ICommand),
            typeof(BottomSheet),
            null);

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public new View Content
    {
        get => (View)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public double MaxHeight
    {
        get => (double)GetValue(MaxHeightProperty);
        set => SetValue(MaxHeightProperty, value);
    }

    public ICommand CloseCommand
    {
        get => (ICommand)GetValue(CloseCommandProperty);
        set => SetValue(CloseCommandProperty, value);
    }

    public BottomSheet()
    {
        InitializeComponent();
    }

    private static void OnIsOpenChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BottomSheet bottomSheet && newValue is bool isOpen)
        {
            if (isOpen)
            {
                bottomSheet.FadeTo(1, 250, Easing.CubicOut);
            }
            else
            {
                bottomSheet.FadeTo(0, 200, Easing.CubicIn);
            }
        }
    }

    public void Open()
    {
        IsOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
    }
}

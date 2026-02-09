namespace LifeSyncApp.Controls;

public partial class LoadingOverlay : ContentView
{
    public static readonly BindableProperty IsVisibleProperty =
        BindableProperty.Create(
            nameof(IsVisible),
            typeof(bool),
            typeof(LoadingOverlay),
            false);

    public static readonly BindableProperty LoadingTextProperty =
        BindableProperty.Create(
            nameof(LoadingText),
            typeof(string),
            typeof(LoadingOverlay),
            "Carregando...");

    public new bool IsVisible
    {
        get => (bool)GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    public string LoadingText
    {
        get => (string)GetValue(LoadingTextProperty);
        set => SetValue(LoadingTextProperty, value);
    }

    public LoadingOverlay()
    {
        InitializeComponent();
    }
}

using System.Windows.Input;

namespace LifeSyncApp.Controls;

public partial class BottomSheet : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(BottomSheet), string.Empty,
            propertyChanged: (b, _, n) =>
            {
                var ctrl = (BottomSheet)b;
                if (ctrl._initialized) ctrl.TitleLabel.Text = (string)n;
            });

    public static readonly BindableProperty CloseCommandProperty =
        BindableProperty.Create(nameof(CloseCommand), typeof(ICommand), typeof(BottomSheet), null,
            propertyChanged: (b, _, n) =>
            {
                var ctrl = (BottomSheet)b;
                if (ctrl._initialized) ctrl.CloseTap.Command = (ICommand)n;
            });

    public static readonly BindableProperty SheetBodyProperty =
        BindableProperty.Create(nameof(SheetBody), typeof(View), typeof(BottomSheet), null,
            propertyChanged: (b, _, n) =>
            {
                var ctrl = (BottomSheet)b;
                if (ctrl._initialized) ctrl.SheetContent.Content = n as View;
            });

    private bool _initialized;

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (SheetContent?.Content != null)
            SheetContent.Content.BindingContext = BindingContext;
    }

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public ICommand CloseCommand { get => (ICommand)GetValue(CloseCommandProperty); set => SetValue(CloseCommandProperty, value); }
    public View SheetBody { get => (View)GetValue(SheetBodyProperty); set => SetValue(SheetBodyProperty, value); }

    public BottomSheet()
    {
        InitializeComponent();
        _initialized = true;

        // Apply any properties that were set before InitializeComponent
        TitleLabel.Text = Title;
        CloseTap.Command = CloseCommand;
        SheetContent.Content = SheetBody;
    }
}
